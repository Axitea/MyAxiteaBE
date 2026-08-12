namespace MYA.Models.Configuration;

public sealed class MailApiOptions
{
    public string SendMailUrl { get; set; } = string.Empty;

    public string BearerToken { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; } = 15;

    public bool UrlEncodeRecipient { get; set; } = true;
}
