namespace AzureNotifier.Functions;

public interface ISendSmsFunction {
    Task<IActionResult> SendSms(HttpRequest req, ILogger log);
}

public class SendSmsFunction : ISendSmsFunction
{
    private ISmsApiService _smsApiService;

    public SendSmsFunction(ISmsApiService smsApiService){
        _smsApiService = smsApiService;
    }

    [FunctionName(nameof(SendSms))]
    public async Task<IActionResult> SendSms(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        string response;

        try {

            var data = JsonConvert.DeserializeObject<NotificationData>(requestBody);

            if(string.IsNullOrEmpty(data?.Message)) {
                log.LogWarning("Failed SendSMS attempt: No Message Content");
                return new BadRequestObjectResult("No Message Content");
            }

            if(string.IsNullOrEmpty(data.MobileNumber)) {
                log.LogWarning("Failed SendSMS attempt: No Mobile Number");
                return new BadRequestObjectResult("No Mobile Number");
            }

            response = _smsApiService.SendSms(data.MobileNumber, data.Message);

        } catch (InvalidOperationException ex) { // Covers ClickSend Api Invalid-Success error cases.
            log.LogError(ex.Message);
            return new BadRequestObjectResult(ex.Message);
        } catch (Exception ex) { 
            log.LogError(ex.ToString());
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        log.LogInformation(response);
        return new OkResult();
    }
}