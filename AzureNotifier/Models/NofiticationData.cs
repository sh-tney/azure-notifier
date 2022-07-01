namespace sh.tney.Models
{
    public class NotificationData
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("messageContent")]
        public string MessageContent;
    }
}