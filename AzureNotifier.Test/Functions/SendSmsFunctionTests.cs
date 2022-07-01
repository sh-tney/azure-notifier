namespace AzureNotifier.Test.Functions;

public class SendSmsFunctionTests 
{
    private Mock<ILogger> mockILogger;
    private Mock<HttpRequest> mockRequest;
    private Mock<ISmsApiService> mockSmsApiService;
    private SendSmsFunction target;

    public SendSmsFunctionTests()
    {
        mockILogger = new Mock<ILogger>();
        mockRequest = new Mock<HttpRequest>();
        mockSmsApiService = new Mock<ISmsApiService>();
        target = new SendSmsFunction(mockSmsApiService.Object);
    }

    private static Mock<HttpRequest> CreateMockRequest(NotificationData body)
    {
        var ms = new MemoryStream();
        var sw = new StreamWriter(ms);

        var json = JsonConvert.SerializeObject(body);

        sw.Write(json);
        sw.Flush();

        ms.Position = 0;

        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(x => x.Body).Returns(ms);

        return mockRequest;
    }

    [Fact]
    public async Task SendSms_ShouldReturnBadRequestObjectResult_WhenNoMessageIsFound()
    {
        mockRequest = CreateMockRequest(new NotificationData { MobileNumber = "test number" });  

        var result = await target.SendSms(mockRequest.Object, mockILogger.Object);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No Message Content", (result as BadRequestObjectResult)?.Value);
    }

    [Fact] 
    public async Task SendSms_ShouldReturnBadRequestObjectResult_WhenNoMobileNumberIsFound()
    {
        mockRequest = CreateMockRequest(new NotificationData { Message = "test message" });

        var result = await target.SendSms(mockRequest.Object, mockILogger.Object);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No Mobile Number", (result as BadRequestObjectResult)?.Value);
    }

    [Fact]
    public async Task SendSms_ShouldReturnBadRequestObjectResult_WhenSmsServiceThrowsInvalidOperationException()
    {
        mockRequest = CreateMockRequest(new NotificationData { MobileNumber = "test number", Message = "test message" });

        mockSmsApiService.Setup(x => x.SendSms(It.IsAny<string>(), It.IsAny<string>())).Throws(new InvalidOperationException("TEST_API_STATUS_CODE"));

        var result = await target.SendSms(mockRequest.Object, mockILogger.Object);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("TEST_API_STATUS_CODE", (result as BadRequestObjectResult)?.Value);
    }

    [Fact]
    public async Task SendSms_ShouldReturnInternalServerErrorStatus_WhenOtherExceptionIsThrown()
    {
        mockRequest = CreateMockRequest(new NotificationData { MobileNumber = "test number", Message = "test message" });

        mockSmsApiService.Setup(x => x.SendSms(It.IsAny<string>(), It.IsAny<string>())).Throws(new FileNotFoundException("Some random exception"));

        var result = await target.SendSms(mockRequest.Object, mockILogger.Object);

        Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, (result as StatusCodeResult)?.StatusCode);
    }

    [Fact]
    public async Task SendSms_ShouldReturnOkResult_WhenMobileNumberAndMessageAreProvided()
    {
        mockRequest = CreateMockRequest(new NotificationData { MobileNumber = "test number", Message = "test message" });

        var result = await target.SendSms(mockRequest.Object, mockILogger.Object);

        Assert.IsType<OkResult>(result);
    }
} 