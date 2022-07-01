namespace AzureNotifier.Models
{
    public class NotificationData
    {
        [JsonProperty("mobile")]
        public string MobileNumber;

        [JsonProperty("message")]
        public string Message;
    }
}