namespace AzureNotifier.Functions;

public interface ISendEmailFunction {
    Task<IActionResult> SendEmail(HttpRequest req, ILogger log);
}

public class SendEmailFunction : ISendEmailFunction
{
    private IEmailApiService _emailApiService;

    public SendEmailFunction(IEmailApiService emailApiService){
        _emailApiService = emailApiService;
    }

    [FunctionName(nameof(SendEmail))]   
    public async Task<IActionResult> SendEmail(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        string response;

        try {

            var data = JsonConvert.DeserializeObject<NotificationData>(requestBody);

            if(string.IsNullOrEmpty(data?.Message)) {
                log.LogWarning("Failed SendEmail attempt: No Message Content");
                return new BadRequestObjectResult("No Message Content");
            }

            if(string.IsNullOrEmpty(data.EmailAddress)) {
                log.LogWarning("Failed SendEmail attempt: No Email Address");
                return new BadRequestObjectResult("No Email Address");
            }

            response = _emailApiService.SendEmail(data.EmailAddress, data.Message, "AzureNotififer Email Test");

        } catch (InvalidOperationException ex) {
            // Covers ClickSend Api Invalid-Success error cases.
            log.LogError(ex.Message);
            return new BadRequestObjectResult(ex.Message);
        } catch (ApiException ex) {
            log.LogError(ex.Message);
            return new BadRequestObjectResult("ClickSend Api Exception");
        } catch (Exception ex) { 
            log.LogError(ex.ToString());
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        log.LogInformation(response);
        return new OkResult();
    }
} 