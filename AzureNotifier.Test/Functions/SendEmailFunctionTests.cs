namespace AzureNotifier.Test.Functions;

public class SendEmailFunctionTests
{
    private Mock<ILogger<SendEmailFunction>> mockILogger;
    private MockHttpRequestData mockRequest;
    private Mock<IEmailApiService> mockEmailApiService;
    private SendEmailFunction target;

    public SendEmailFunctionTests()
    {
        mockILogger = new Mock<ILogger<SendEmailFunction>>();
        mockRequest = new MockHttpRequestData(new Mock<FunctionContext>().Object, new Uri("http://google.com"));
        mockEmailApiService = new Mock<IEmailApiService>();
        target = new SendEmailFunction(mockEmailApiService.Object, mockILogger.Object);
    }

    [Fact]
    public async Task SendEmail_ShouldReturnBadRequestObjectResult_WhenNoMessageIsFound()
    {
        mockRequest = TestHelpers.CreateMockRequest(new NotificationData { EmailAddress = "test address" });  

        var result = await target.SendEmail(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.BadRequest, "No Message Content");
    }

    [Fact] 
    public async Task SendEmail_ShouldReturnBadRequestObjectResult_WhenNoEmailAddressIsFound()
    {
        mockRequest = TestHelpers.CreateMockRequest(new NotificationData { Message = "test message" });

        var result = await target.SendEmail(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.BadRequest, "No Email Address");
    }

    [Fact]
    public async Task SendEmail_ShouldReturnBadRequestObjectResult_WhenEmailServiceThrowsInvalidOperationException()
    {
        mockRequest = TestHelpers.CreateMockRequest(new NotificationData { Message = "test message", EmailAddress = "test address" });
        mockEmailApiService.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new InvalidOperationException("TEST_API_STATUS_CODE"));

        var result = await target.SendEmail(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.BadRequest, "TEST_API_STATUS_CODE");
    }

    [Fact]
    public async Task SendEmail_ShouldReturnBadRequestObjectResult_WhenEmailServiceThrowsApiException()
    {
        mockRequest = TestHelpers.CreateMockRequest(new NotificationData { Message = "test message", EmailAddress = "test address" });
        mockEmailApiService.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new ApiException());

        var result = await target.SendEmail(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.BadRequest, "ClickSend Api Exception");
    }

    [Fact]
    public async Task SendEmail_ShouldReturnBadRequestObjectResult_WhenJSONIsInvalid()
    {
        var mockRequest = TestHelpers.CreateMockRequest("{\"email\": \"test@xyz.com\" \"message\": \"test message\"}");

        var result = await target.SendEmail(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.BadRequest, "Invalid JSON");
    }

    [Fact]
    public async Task SendEmail_ShouldReturnInternalServerErrorStatus_WhenOtherExceptionIsThrown()
    {
        mockRequest = TestHelpers.CreateMockRequest(new NotificationData { Message = "test message", EmailAddress = "test address" });
        mockEmailApiService.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new FileNotFoundException("Some random exception"));

        var result = await target.SendEmail(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.InternalServerError, null);
    }

    [Fact]
    public async Task SendEmail_ShouldReturnOkResult_WhenMessageAndEmailAddressAreProvided()
    {
        mockRequest = TestHelpers.CreateMockRequest(new NotificationData { Message = "test message" , EmailAddress = "test address" });

        var result = await target.SendEmail(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.OK, null);
    }
} 