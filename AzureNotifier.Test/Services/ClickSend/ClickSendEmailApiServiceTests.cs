namespace AzureNotifier.Test.Services.ClickSend;

public class ClickSendEmailApiServiceTests
{
    private Mock<IClickSendEmailApiWrapper> mockClickSendEmailApiWrapper;
    private ClickSendEmailApiService target;

    public ClickSendEmailApiServiceTests()
    {
        mockClickSendEmailApiWrapper = new Mock<IClickSendEmailApiWrapper>();
        target = new ClickSendEmailApiService(mockClickSendEmailApiWrapper.Object);

        Environment.SetEnvironmentVariable("CLICKSEND_EMAIL_ADDRESS_ID", "test id");
    }

    [Fact]
    public void ClickSendEmailApiService_ShouldReturnApiResponse_WhenApiResponseContainsSuccessCode()
    {
        var sampleValidSuccess = @"{""http_code"":200,""response_code"":""SUCCESS"",""response_msg"":""Transactional email queued for delivery."",""data"":{""user_id"":289253,""subaccount_id"":328627,""from_email_address_id"":""19653"",""from_name"":""AzureNotifierTest"",""to"":[{""email"":""xyz@abc.com""}],""cc"":null,""bcc"":null,""subject"":""AzureNotififer Email Test"",""body"":""Test email message"",""body_plain_text"":""Test email message"",""schedule"":1642541056,""message_id"":""74F81E67-1381-4D6F-B964-83B9B4FC6AB3"",""status"":""Queued"",""status_text"":""Accepted for delivery"",""soft_bounce_count"":0,""hard_bounce_count"":0,""price"":""0.0106"",""date_added"":1642541056,""custom_string"":null,""_attachments"":[],""_currency"":{""currency_name_short"":""NZD"",""currency_prefix_d"":""$"",""currency_prefix_c"":""c"",""currency_name_long"":""New Zealand Dollars"",""min_recharge_amount"":""20.00"",""max_recharge_amount"":""6000.00""},""_api_username"":""abc@xyz.com""}}";

        mockClickSendEmailApiWrapper.Setup(x => x.EmailSendPost(It.IsAny<Email>())).Returns(sampleValidSuccess);

        var result = target.SendEmail("test number", "test message", "test subject");

        Assert.Equal(sampleValidSuccess, result);
    }

    [Fact]
    public void ClickSendEmailApiService_ThrowInvalidOperationExceptionWithFailCode_WhenApiResponseContainsFailCode()
    {
        var sampleInvalidSuccess = @"{""http_code"":200,""response_code"":""TEST_RESPONSE_CODE"",""response_msg"":""Transactional email queued for delivery."",""data"":{""user_id"":289253,""subaccount_id"":328627,""from_email_address_id"":""19653"",""from_name"":""AzureNotifierTest"",""to"":[{""email"":""xyz@abc.com""}],""cc"":null,""bcc"":null,""subject"":""AzureNotififer Email Test"",""body"":""Test email message"",""body_plain_text"":""Test email message"",""schedule"":1642541056,""message_id"":""74F81E67-1381-4D6F-B964-83B9B4FC6AB3"",""status"":""Queued"",""status_text"":""Accepted for delivery"",""soft_bounce_count"":0,""hard_bounce_count"":0,""price"":""0.0106"",""date_added"":1642541056,""custom_string"":null,""_attachments"":[],""_currency"":{""currency_name_short"":""NZD"",""currency_prefix_d"":""$"",""currency_prefix_c"":""c"",""currency_name_long"":""New Zealand Dollars"",""min_recharge_amount"":""20.00"",""max_recharge_amount"":""6000.00""},""_api_username"":""abc@xyz.com""}}";

        mockClickSendEmailApiWrapper.Setup(x => x.EmailSendPost(It.IsAny<Email>())).Returns(sampleInvalidSuccess);

        var exception = Assert.Throws<InvalidOperationException>(() => target.SendEmail("test number", "test message", "test subject"));

        Assert.Equal("TEST_RESPONSE_CODE", exception.Message);
    }
} 