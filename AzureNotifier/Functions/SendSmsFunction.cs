namespace AzureNotifier.Functions;

public interface ISendSmsFunction {
    Task<HttpResponseData> SendSms(HttpRequestData req);
}

public class SendSmsFunction : ISendSmsFunction
{
    private readonly ISmsApiService _smsApiService;
    private readonly ILogger<SendSmsFunction> _logger;

    public SendSmsFunction(ISmsApiService smsApiService, ILogger<SendSmsFunction> logger){
        _smsApiService = smsApiService;
        _logger = logger;
    }

    [Function(nameof(SendSms))]
    public async Task<HttpResponseData> SendSms(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        string response;

        try {

            var data = JsonConvert.DeserializeObject<NotificationData>(requestBody);

            if(string.IsNullOrEmpty(data?.Message)) {
                _logger.LogWarning("Failed SendSMS attempt: No Message Content");
                var resp = req.CreateResponse(HttpStatusCode.BadRequest);
                resp.WriteString("No Message Content");
                return resp;
            }

            if(string.IsNullOrEmpty(data.MobileNumber)) {
                _logger.LogWarning("Failed SendSMS attempt: No Mobile Number");
                var resp = req.CreateResponse(HttpStatusCode.BadRequest);
                resp.WriteString("No Mobile Number");
                return resp;
            }

            response = _smsApiService.SendSms(data.MobileNumber, data.Message);

        } catch (JsonReaderException ex) {
            _logger.LogWarning(ex.Message);
            var resp = req.CreateResponse(HttpStatusCode.BadRequest);
            resp.WriteString("Invalid JSON");
            return resp;
        } catch (InvalidOperationException ex) { // Covers ClickSend Api Invalid-Success error cases.
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