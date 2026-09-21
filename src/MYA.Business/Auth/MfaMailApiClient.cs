using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using MYA.Models.Auth;
using MYA.Models.Configuration;

namespace MYA.Business.Auth;

public sealed class MfaMailApiClient
{
    private readonly HttpClient _httpClient;
    private readonly MailApiOptions _options;

    public MfaMailApiClient(HttpClient httpClient, IOptions<MailApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task SendMfaCodeAsync(
        MfaMailMessage message,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.SendMailUrl))
        {
            throw new InvalidOperationException("Mail API URL non configurata.");
        }

        var recipient = _options.UrlEncodeRecipient
            ? Uri.EscapeDataString(message.To)
            : message.To;

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.SendMailUrl);
        request.Headers.TryAddWithoutValidation("Accept", "application/json");
        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_options.BearerToken}");
        request.Content = JsonContent.Create(new SendMailRequest(
            recipient,
            message.Subject,
            message.Body,
            message.SenderDescription,
            HasCC: false,
            CC: string.Empty,
            BCC: string.Empty));

        using var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Invio mail MFA non riuscito. StatusCode={(int)response.StatusCode}.");
        }
    }

    private sealed record SendMailRequest(
        string To,
        string Subject,
        string Body,
        string DescrSender,
        bool HasCC,
        string CC,
        string BCC);
}
