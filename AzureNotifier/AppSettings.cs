namespace AzureNotifier;

public class AppSettings
{
    // ClickSend API Variables
    public string ClickSendUsername { get; set; }
    public string ClickSendPassword { get; set; }
    public string ClickSendFromEmailId { get; set; }

    // Twilio API Variables
    public string TwilioAccountSid { get; set; }
    public string TwilioAuthToken { get; set; }
    public string TwilioFromNumber { get; set; }
} 