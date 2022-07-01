namespace AzureNotifier.Services.ClickSend;

public interface IClickSendEmailApiWrapper {
    string EmailSendPost(Email email);
}

public class ClickSendEmailApiWrapper : IClickSendEmailApiWrapper {
    private static readonly TransactionalEmailApi _clickSendEmailApi = new TransactionalEmailApi(Globals.clickSendConfig);

    public string EmailSendPost(Email email) {
        return _clickSendEmailApi.EmailSendPost(email);
    }
}

public class ClickSendEmailApiService : IEmailApiService {
    private IClickSendEmailApiWrapper _emailApi;

    public ClickSendEmailApiService(IClickSendEmailApiWrapper clickSendEmailApiWrapper){
        _emailApi = clickSendEmailApiWrapper;
    }

    public string SendEmail(string emailAddress, string message, string subject)
    {
        var email = new Email(
            to: new List<EmailRecipient>{ new EmailRecipient(emailAddress) },
            from: new EmailFrom(Globals.ClickSendEmailFromAddressId, "AzureNotifierTest"),
            subject: subject,
            body: message
        );

        var response = _emailApi.EmailSendPost(email);

        // ClickSend has its own list of internal message status codes, which are not necessarily reflected in the HTTP status code.
        // These codes can be found here: https://developers.clicksend.com/docs/rest/v3/?csharp#application-status-codes
        dynamic responseObj = JsonConvert.DeserializeObject<dynamic>(response);
        string messageStatus = responseObj.response_code;

        if (messageStatus != "SUCCESS") {
            throw new InvalidOperationException(messageStatus);
        }

        return response;
    }
} 