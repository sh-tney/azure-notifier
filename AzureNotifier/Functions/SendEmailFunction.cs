namespace AzureNotifier.Functions;

public interface ISendEmailFunction {
    Task<HttpResponseData> SendEmail(HttpRequestData req);
}

public class SendEmailFunction : ISendEmailFunction
{
    private readonly IEmailApiService _emailApiService;
    private readonly ILogger<SendEmailFunction> _logger;

    public SendEmailFunction(
        IEmailApiService emailApiService, 
        ILogger<SendEmailFunction> logger)
    {
        _emailApiService = emailApiService;
        _logger = logger;
    }

    [Function(nameof(SendEmail))]   
    public async Task<HttpResponseData> SendEmail(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        string response;

        try {

            var data = JsonConvert.DeserializeObject<NotificationData>(requestBody);

            if(string.IsNullOrEmpty(data?.Message)) {
                _logger.LogWarning("Failed SendEmail attempt: No Message Content");
                var resp = req.CreateResponse(HttpStatusCode.BadRequest);
                resp.WriteString("No Message Content");
                return resp;
            }

            if(string.IsNullOrEmpty(data.EmailAddress)) {
                _logger.LogWarning("Failed SendEmail attempt: No Email Address");
                var resp = req.CreateResponse(HttpStatusCode.BadRequest);
                resp.WriteString("No Email Address");
                return resp;
            }

            response = _emailApiService.SendEmail(data.EmailAddress, data.Message, "AzureNotififer Email Test");

        } catch (JsonReaderException ex) {
            _logger.LogWarning(ex.Message);
            var resp = req.CreateResponse(HttpStatusCode.BadRequest);
            resp.WriteString("Invalid JSON");
            return resp;
        } catch (JsonSerializationException ex) {
            _logger.LogWarning(ex.Message);
            var resp = req.CreateResponse(HttpStatusCode.BadRequest);
            resp.WriteString("Invalid JSON");
            return resp;
        } catch (InvalidOperationException ex) {
            // Covers ClickSend Api Invalid-Success error cases.
            _logger.LogError(ex.Message);
            var resp = req.CreateResponse(HttpStatusCode.BadRequest);
            resp.WriteString(ex.Message);
            return resp;
        } catch (ApiException ex) {
            _logger.LogError(ex.Message);
            var resp = req.CreateResponse(HttpStatusCode.BadRequest);
            resp.WriteString("ClickSend Api Exception");
            return resp;
        } catch (Exception ex) { 
            _logger.LogError(ex.ToString());
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }

        _logger.LogInformation(response);
        return req.CreateResponse(HttpStatusCode.OK);
    }
} 