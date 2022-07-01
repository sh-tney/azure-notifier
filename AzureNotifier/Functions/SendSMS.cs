namespace AzureNotifier.Functions;

public static class SendSMS
{
    private static string GenerateSMSMessage(string name, string messageContent){
        return $"Dear {name}: \n {messageContent}.";
    }

    [FunctionName("SendSMS")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        string response;

        try {
            var data = JsonConvert.DeserializeObject<NotificationData>(requestBody);

            if(string.IsNullOrEmpty(data?.MessageContent)) {
                log.LogWarning("Failed SendNotification attempt: No Message Content");
                return new BadRequestObjectResult("No Message Content");
            }

            if(string.IsNullOrEmpty(data.Name)) {
                log.LogWarning("Failed SendNotification attempt: No Name");
                return new BadRequestObjectResult("No Name");
            }

            var x = new List<SmsMessage> {
                new SmsMessage(
                    // +14055555555 is a ClickSend test number which has no cost, and will always return a success code.
                    to: "+14055555555",
                    body: GenerateSMSMessage(data.Name, data.MessageContent),                
                    source: "sdk"
                )
            };

            var smsCollection = new SmsMessageCollection(x);
            response = Globals.clickSendSMSApi.SmsSendPost(smsCollection);

        } catch (Exception ex) {
            log.LogError(ex.ToString());
            return new BadRequestObjectResult(ex.ToString());
        }

        log.LogInformation(response);
        return new OkObjectResult(response);
    }
}