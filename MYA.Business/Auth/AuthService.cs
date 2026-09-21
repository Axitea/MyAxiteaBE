using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MYA.Data.Puzzle;
using MYA.Data.Sat;
using MYA.Models.Auth;
using MYA.Models.Configuration;

namespace MYA.Business.Auth;

public sealed class AuthService
{
    private const int SmsMaxLength = 160;
    private const string DeliveryChannelSms = "sms";
    private const string DeliveryChannelEmail = "email";
    private const string DeliveryChannelNone = "none";

    private readonly PuzzleDataAccess _puzzleDataAccess;
    private readonly SatDataAccess _satDataAccess;
    private readonly MfaMailApiClient _mfaMailApiClient;
    private readonly LegacyPasswordCipher _passwordCipher;
    private readonly MfaCodeGenerator _mfaCodeGenerator;
    private readonly JwtTokenService _jwtTokenService;
    private readonly MyAuthOptions _authOptions;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        PuzzleDataAccess puzzleDataAccess,
        SatDataAccess satDataAccess,
        MfaMailApiClient mfaMailApiClient,
        LegacyPasswordCipher passwordCipher,
        MfaCodeGenerator mfaCodeGenerator,
        JwtTokenService jwtTokenService,
        IOptions<MyAuthOptions> authOptions,
        TimeProvider timeProvider,
        ILogger<AuthService> logger)
    {
        _puzzleDataAccess = puzzleDataAccess;
        _satDataAccess = satDataAccess;
        _mfaMailApiClient = mfaMailApiClient;
        _passwordCipher = passwordCipher;
        _mfaCodeGenerator = mfaCodeGenerator;
        _jwtTokenService = jwtTokenService;
        _authOptions = authOptions.Value;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<LoginStartedResponse> StartLoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var encryptedPassword = _passwordCipher.Encrypt(request.Password, _authOptions.LegacyPasswordKeyPhrase);

        var user = await _puzzleDataAccess.GetLoginAsync(
                request.CodiceCliente.Trim(),
                request.Login.Trim(),
                encryptedPassword,
                _authOptions.AppId,
                cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
        {
            throw new AuthDomainException("Credenziali non valide.");
        }

        EnsureLoginCanContinue(user);
        var delivery = ResolveMfaDelivery(user);

        var now = _timeProvider.GetUtcNow();
        if (delivery is null)
        {
            var token = await IssueTokenAsync(user, now, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogWarning(
                "Login senza MFA consentito: account privo di canali 2FA. UserIdToken={UserIdToken} Login={Login} AppId={AppId}",
                user.UserIdToken,
                user.Login,
                _authOptions.AppId);

            return new LoginStartedResponse(
                user.UserIdToken,
                user.Login,
                user.DisplayName,
                MaskedPhoneNumber: null,
                MfaDeliveryChannel: DeliveryChannelNone,
                MaskedDestination: null,
                MfaExpiresAt: null,
                RequiresMfa: false,
                Token: token,
                SecurityNotice: _authOptions.MissingMfaSecurityNotice);
        }

        EnsureMfaCanStart(user);

        var code = _mfaCodeGenerator.Generate(_authOptions.MfaCodeLength);

        await _puzzleDataAccess.SaveMfaCodeAsync(user.UserIdToken, _authOptions.AppId, code, now, cancellationToken)
            .ConfigureAwait(false);

        if (delivery.Channel == DeliveryChannelSms)
        {
            var smsText = BuildSmsText(code);
            await _satDataAccess.EnqueueMfaSmsAsync(
                    new MfaSmsMessage(user.UserIdToken, delivery.Destination, smsText, now),
                    cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            await SendMfaMailAsync(user, delivery.Destination, code, now, cancellationToken)
                .ConfigureAwait(false);
        }

        return new LoginStartedResponse(
            user.UserIdToken,
            user.Login,
            user.DisplayName,
            MaskPhone(user.PhoneNumber),
            delivery.Channel,
            delivery.MaskedDestination,
            now.AddMinutes(_authOptions.MfaTtlMinutes),
            RequiresMfa: true,
            Token: null,
            SecurityNotice: null);
    }

    public async Task<TokenResponse> VerifyMfaAsync(
        VerifyMfaRequest request,
        CancellationToken cancellationToken = default)
    {
        var now = _timeProvider.GetUtcNow();
        var user = await _puzzleDataAccess.VerifyMfaCodeAsync(
                request.UserId,
                _authOptions.AppId,
                request.Code,
                now,
                _authOptions.MfaTtlMinutes,
                cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
        {
            throw new AuthDomainException("Codice MFA non valido o scaduto.");
        }

        return await IssueTokenAsync(user, now, cancellationToken)
            .ConfigureAwait(false);
    }

    private static void EnsureLoginCanContinue(LoginUser user)
    {
        if (user.IsMfaLocked)
        {
            throw new AuthDomainException("Account bloccato per MFA.");
        }
    }

    private static void EnsureMfaCanStart(LoginUser user)
    {
        if (!user.IsMfaActive)
        {
            throw new AuthDomainException("MFA non attiva per l'utente.");
        }
    }

    private static MfaDelivery? ResolveMfaDelivery(LoginUser user)
    {
        if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
        {
            return new MfaDelivery(DeliveryChannelSms, user.PhoneNumber.Trim(), MaskPhone(user.PhoneNumber));
        }

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            return new MfaDelivery(DeliveryChannelEmail, user.Email.Trim(), MaskEmail(user.Email));
        }

        return null;
    }

    private async Task<TokenResponse> IssueTokenAsync(
        LoginUser user,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var issuedToken = _jwtTokenService.Issue(user, _authOptions.AppId, now);

        await _puzzleDataAccess.SaveIssuedTokenAsync(
                new JwtTokenWrite(
                    issuedToken.AccessToken,
                    user.Login,
                    issuedToken.RefreshToken,
                    _authOptions.AppId),
                cancellationToken)
            .ConfigureAwait(false);

        return new TokenResponse(
            issuedToken.AccessToken,
            issuedToken.RefreshToken,
            issuedToken.AccessTokenExpiresAt,
            user.UserId,
            user.Login,
            _authOptions.AppId);
    }

    private string BuildSmsText(string code)
    {
        var text = string.Format(_authOptions.SmsTextTemplate, code);
        return text.Length <= SmsMaxLength ? text : text[..SmsMaxLength];
    }

    private async Task SendMfaMailAsync(
        LoginUser user,
        string email,
        string code,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var message = new MfaMailMessage(
            user.UserIdToken,
            email,
            _authOptions.MailSubject,
            string.Format(_authOptions.MailTextTemplate, code),
            _authOptions.MailSenderDescription,
            now);

        try
        {
            await _mfaMailApiClient.SendMfaCodeAsync(message, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested && IsMailDeliveryException(exception))
        {
            throw new AuthDomainException("Non e stato possibile inviare il codice MFA via email.");
        }
    }

    private static bool IsMailDeliveryException(Exception exception)
    {
        return exception is HttpRequestException or InvalidOperationException or TaskCanceledException;
    }

    private static string? MaskPhone(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return null;
        }

        var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());
        if (digits.Length <= 4)
        {
            return "****";
        }

        return $"{new string('*', Math.Max(0, digits.Length - 4))}{digits[^4..]}";
    }

    private static string? MaskEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var trimmed = email.Trim();
        var atIndex = trimmed.IndexOf('@', StringComparison.Ordinal);
        if (atIndex <= 1)
        {
            return $"***{trimmed[atIndex..]}";
        }

        var local = trimmed[..atIndex];
        var domain = trimmed[atIndex..];
        return $"{local[0]}{new string('*', Math.Min(6, local.Length - 1))}{domain}";
    }

    private sealed record MfaDelivery(string Channel, string Destination, string? MaskedDestination);
}
