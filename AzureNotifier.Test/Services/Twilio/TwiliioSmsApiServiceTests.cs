namespace AzureNotifier.Test.Services.Twilio;

public class TwilioSmsApiServiceTests
{
    private Mock<ITwilioSmsApiWrapper> mockTwilioSmsApiWrapper;
    private TwilioSmsApiService target;

    public TwilioSmsApiServiceTests()
    {
        mockTwilioSmsApiWrapper = new Mock<ITwilioSmsApiWrapper>();
        target = new TwilioSmsApiService(mockTwilioSmsApiWrapper.Object);
    }

    [Fact]
    public void TwilioSmsApiService_ShouldReturnApiResponse_WhenApiResponseSuccessful()
    {
        var sampleValidSuccess = @"{""body"":""Sent from your Twilio trial account - Hey"",""num_segments"":""1"",""direction"":""outbound-api"",""from"":""+1111111"",""to"":""111111111111111"",""date_updated"":""2022-07-03T23:28:49+12:00"",""price"":null,""error_message"":null,""uri"":""/2010-04-01/Accounts/12312312312312312312/Messages/SM9dc7804126bf4bd98012c8affec1a518.json"",""account_sid"":""123123123123123"",""num_media"":""0"",""status"":""queued"",""messaging_service_sid"":null,""sid"":""SM9dc7804126bf4bd98012c8affec1a518"",""date_sent"":null,""date_created"":""2022-07-03T23:28:49+12:00"",""error_code"":null,""price_unit"":""USD"",""api_version"":""2010-04-01"",""subresource_uris"":{""media"":""/2010-04-01/Accounts/123123123123123123/Messages/SM9dc7804126bf4bd98012c8affec1a518/Media.json""}}";
        mockTwilioSmsApiWrapper.Setup(x => x.SmsSendPost(It.IsAny<string>(), It.IsAny<string>())).Returns(sampleValidSuccess);

        var result = target.SendSms("test number", "test message");

        Assert.Equal(sampleValidSuccess, result);
    }
} 