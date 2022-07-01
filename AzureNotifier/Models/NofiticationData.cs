namespace AzureNotifier.Models;

/// <summary>
    /// Model acts as a catch-all structure for all Notification requests
/// </summary>
public class NotificationData
{
    [JsonProperty("mobile")]
    public string MobileNumber;

    [JsonProperty("email")]
    public string EmailAddress;

    [JsonProperty("message")]
    public string Message;
}