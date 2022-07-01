namespace AzureNotifier.Services.ClickSend;

public interface IClickSendSmsApiWrapper {
    string SmsSendPost(SmsMessageCollection smsMessageCollection);
}

public class ClickSendSmsApiWrapper : IClickSendSmsApiWrapper {
    private static readonly SMSApi _clickSendSMSApi = new SMSApi(Globals.clickSendConfig);

    public string SmsSendPost(SmsMessageCollection smsMessageCollection) {
        return _clickSendSMSApi.SmsSendPost(smsMessageCollection);
    }
}

public class ClickSendSmsApiService : ISmsApiService {
    private IClickSendSmsApiWrapper _smsApi;

    public ClickSendSmsApiService(IClickSendSmsApiWrapper clickSendSmsApiWrapper){
        _smsApi = clickSendSmsApiWrapper;
    }

    public string SendSms(string mobileNumber, string message)
    {
        var smsCollection = new SmsMessageCollection(new List<SmsMessage> {
            new SmsMessage(
                // +14055555555 is a ClickSend test number which has no cost, and will always return a success code.
                to: mobileNumber,
                body: message,                
                source: "sdk"
            )
        });

        var response = _smsApi.SmsSendPost(smsCollection);

        // ClickSend has its own list of internal message status codes, which are not necessarily reflected in the HTTP status code.
        // These codes can be found here: https://developers.clicksend.com/docs/rest/v3/?csharp#application-status-codes
        dynamic responseObj = JsonConvert.DeserializeObject<dynamic>(response);
        string messageStatus = responseObj.data.messages[0].status;

        if (messageStatus != "SUCCESS") {
            throw new InvalidOperationException(messageStatus);
        }

        return response;
    }
} 