namespace AzureNotifier.Test.Functions;

public class SendEmailFunctionTests
{
    private Mock<ILogger> mockILogger;
    private Mock<HttpRequest> mockRequest;
    private Mock<IEmailApiService> mockEmailApiService;
    private SendEmailFunction target;

    public SendEmailFunctionTests()
    {
        mockILogger = new Mock<ILogger>();
        mockRequest = new Mock<HttpRequest>();
        mockEmailApiService = new Mock<IEmailApiService>();
        target = new SendEmailFunction(mockEmailApiService.Object);
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
    public async Task SendEmail_ShouldReturnBadRequestObjectResult_WhenNoTokenIsFound()
    {
        mockRequest = CreateMockRequest(new NotificationData { EmailAddress = "test address" });  

        var result = await target.SendEmail(mockRequest.Object, mockILogger.Object);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No Message Content", (result as BadRequestObjectResult)?.Value);
    }

    [Fact] 
    public async Task SendEmail_ShouldReturnBadRequestObjectResult_WhenNoEmailAddressIsFound()
    {
        mockRequest = CreateMockRequest(new NotificationData { Message = "test message" });

        var result = await target.SendEmail(mockRequest.Object, mockILogger.Object);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No Email Address", (result as BadRequestObjectResult)?.Value);
    }

    [Fact]
    public async Task SendEmail_ShouldReturnBadRequestObjectResult_WhenEmailServiceThrowsInvalidOperationException()
    {
        mockRequest = CreateMockRequest(new NotificationData { Message = "test message", EmailAddress = "test address" });

        mockEmailApiService.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InvalidOperationException("TEST_API_STATUS_CODE"));

        var result = await target.SendEmail(mockRequest.Object, mockILogger.Object);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("TEST_API_STATUS_CODE", (result as BadRequestObjectResult)?.Value);
    }

    [Fact]
    public async Task SendEmail_ShouldReturnBadRequestObjectResult_WhenEmailServiceThrowsApiException()
    {
        mockRequest = CreateMockRequest(new NotificationData { Message = "test message", EmailAddress = "test address" });

        mockEmailApiService.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new ApiException());

        var result = await target.SendEmail(mockRequest.Object, mockILogger.Object);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ClickSend Api Exception", (result as BadRequestObjectResult)?.Value);
    }

    [Fact]
    public async Task SendEmail_ShouldReturnBadRequestObjectResult_WhenJSONIsInvalid()
    {
        var ms = new MemoryStream();
        var sw = new StreamWriter(ms);

        sw.Write("{\"email\": \"test@xyz.com\" \"message\": \"test message\"}");
        sw.Flush();
        ms.Position = 0;

        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(x => x.Body).Returns(ms);

        var result = await target.SendEmail(mockRequest.Object, mockILogger.Object);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid JSON", (result as BadRequestObjectResult)?.Value);
    }

    [Fact]
    public async Task SendEmail_ShouldReturnInternalServerErrorStatus_WhenOtherExceptionIsThrown()
    {
        mockRequest = CreateMockRequest(new NotificationData { Message = "test message", EmailAddress = "test address" });

        mockEmailApiService.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new FileNotFoundException("Some random exception"));

        var result = await target.SendEmail(mockRequest.Object, mockILogger.Object);

        Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, (result as StatusCodeResult)?.StatusCode);
    }

    [Fact]
    public async Task SendEmail_ShouldReturnOkResult_WhenTokenAndSmsNumberAreProvided()
    {
        mockRequest = CreateMockRequest(new NotificationData { Message = "test message" , EmailAddress = "test address" });

        var result = await target.SendEmail(mockRequest.Object, mockILogger.Object);

        Assert.IsType<OkResult>(result);
    }
} 