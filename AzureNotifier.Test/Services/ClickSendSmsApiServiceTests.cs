namespace AzureNotifier.Test.Services;

public class ClickSendSmsApiServiceTests
{
    private Mock<IClickSendSmsApiWrapper> mockClickSendSmsApiWrapper;
    private ClickSendSmsApiService target;

    public ClickSendSmsApiServiceTests()
    {
        mockClickSendSmsApiWrapper = new Mock<IClickSendSmsApiWrapper>();
        target = new ClickSendSmsApiService(mockClickSendSmsApiWrapper.Object);
    }

    [Fact]
    public void ClickSendSmsApiService_ShouldReturnApiResponse_WhenApiResponseContainsSuccessCode()
    {
        var sampleValidSuccess = @"{""http_code"":200,""response_code"":""SUCCESS"",""response_msg"":""Messages queued for delivery."",""data"":{""total_price"":0,""total_count"":1,""queued_count"":0,""messages"":[{""to"":""+14055555555"",""body"":""test message"",""from"":"""",""schedule"":0,""message_id"":""8D34EE7D-EBA3-47D8-B094-5A6ECBC652F2"",""custom_string"":"""",""status"":""SUCCESS""}],""_currency"":{""currency_name_short"":""NZD"",""currency_prefix_d"":""$"",""currency_prefix_c"":""c"",""currency_name_long"":""New Zealand Dollars""}}}";

        mockClickSendSmsApiWrapper.Setup(x => x.SmsSendPost(It.IsAny<SmsMessageCollection>())).Returns(sampleValidSuccess);

        var result = target.SendSms("test number", "test message");

        Assert.Equal(sampleValidSuccess, result);
    }

    [Fact]
    public void ClickSendSmsApiService_ThrowInvalidOperationExceptionWithFailCode_WhenSmsApiResponseContainsFailCode()
    {
        var sampleInvalidSuccess = @"{""http_code"":200,""response_code"":""SUCCESS"",""response_msg"":""Messages queued for delivery."",""data"":{""total_price"":0,""total_count"":1,""queued_count"":0,""messages"":[{""to"":""111111111111111"",""body"":""test message"",""from"":"""",""schedule"":0,""message_id"":""8D34EE7D-EBA3-47D8-B094-5A6ECBC652F2"",""custom_string"":"""",""status"":""TEST_RESPONSE_CODE""}],""_currency"":{""currency_name_short"":""NZD"",""currency_prefix_d"":""$"",""currency_prefix_c"":""c"",""currency_name_long"":""New Zealand Dollars""}}}";

        mockClickSendSmsApiWrapper.Setup(x => x.SmsSendPost(It.IsAny<SmsMessageCollection>())).Returns(sampleInvalidSuccess);

        var exception = Assert.Throws<InvalidOperationException>(() => target.SendSms("test number", "test message"));
        Assert.Equal("TEST_RESPONSE_CODE", exception.Message);
    }
} 