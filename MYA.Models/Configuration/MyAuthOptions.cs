namespace MYA.Models.Configuration;

public sealed class MyAuthOptions
{
    public int AppId { get; set; } = 2;

    public string LegacyPasswordKeyPhrase { get; set; } = string.Empty;

    public int MfaCodeLength { get; set; } = 6;

    public int MfaTtlMinutes { get; set; } = 5;

    public string SmsTextTemplate { get; set; } = "Il tuo codice di sicurezza e' {0}. Non condividerlo.";

    public string MailSubject { get; set; } = "Codice di sicurezza MYA";

    public string MailTextTemplate { get; set; } = "Il tuo codice di sicurezza e' {0}. Non condividerlo.";

    public string MailSenderDescription { get; set; } = "PUZZLE";

    public string MissingMfaSecurityNotice { get; set; } =
        "Account sprovvisto di sicurezza a due fattori: nessun cellulare o indirizzo email MFA configurato.";
}
