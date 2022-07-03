namespace AzureNotifier.Services.Twilio;

public interface ITwilioSmsApiWrapper {
    MessageResource SmsSendPost(string message, string toNumber);
}

public class TwilioSmsApiWrapper : ITwilioSmsApiWrapper {
    private readonly string _twilioFromNumber;

    public TwilioSmsApiWrapper(IOptions<AppSettings> settings){
        TwilioClient.Init(settings.Value.TwilioAccountSid, settings.Value.TwilioAuthToken);
        _twilioFromNumber = settings.Value.TwilioFromNumber;
    }
    
    public MessageResource SmsSendPost(string mobileNumber, string message) {
        var response = MessageResource.Create(
            body: message,
            from: new PhoneNumber(_twilioFromNumber),
            to: new PhoneNumber(mobileNumber)
        );

        return response;
    }
}

public class TwilioSmsApiService : ISmsApiService {
    private ITwilioSmsApiWrapper _smsApi;

    public TwilioSmsApiService(ITwilioSmsApiWrapper twilioSmsApiWrapper){
        _smsApi = twilioSmsApiWrapper;
    }

    public string SendSms(string mobileNumber, string message)
    {
        var response = _smsApi.SmsSendPost(mobileNumber, message);

        return JsonConvert.SerializeObject(response);
    }
} 