namespace AzureNotifier.Test.Functions;

public class SendSmsFunctionTests 
{
    private Mock<ILogger<SendSmsFunction>> mockILogger;
    private MockHttpRequestData mockRequest;
    private Mock<ISmsApiService> mockSmsApiService;
    private SendSmsFunction target;

    public SendSmsFunctionTests()
    {
        mockILogger = new Mock<ILogger<SendSmsFunction>>();
        mockRequest = new MockHttpRequestData(new Mock<FunctionContext>().Object, new Uri("http://google.com"));
        mockSmsApiService = new Mock<ISmsApiService>();
        target = new SendSmsFunction(mockSmsApiService.Object, mockILogger.Object);
    }

    [Fact]
    public async Task SendSms_ShouldReturnBadRequestObjectResult_WhenNoMessageIsFound()
    {
        mockRequest = TestHelpers.CreateMockRequest(new NotificationData { MobileNumber = "test number" });  

        var result = await target.SendSms(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.BadRequest, "No Message Content");
    }

    [Fact] 
    public async Task SendSms_ShouldReturnBadRequestObjectResult_WhenNoMobileNumberIsFound()
    {
        mockRequest = TestHelpers.CreateMockRequest(new NotificationData { Message = "test message" });

        var result = await target.SendSms(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.BadRequest, "No Mobile Number");
    }

    [Fact]
    public async Task SendSms_ShouldReturnBadRequestObjectResult_WhenSmsServiceThrowsInvalidOperationException()
    {
        mockRequest = TestHelpers.CreateMockRequest(new NotificationData { MobileNumber = "test number", Message = "test message" });

        mockSmsApiService.Setup(x => x.SendSms(It.IsAny<string>(), It.IsAny<string>())).Throws(new InvalidOperationException("TEST_API_STATUS_CODE"));

        var result = await target.SendSms(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.BadRequest, "TEST_API_STATUS_CODE");
    }

    [Fact]
    public async Task SendSms_ShouldReturnBadRequestObjectResult_WhenSmsServiceThrowsApiException()
    {
        mockRequest = TestHelpers.CreateMockRequest(new NotificationData { MobileNumber = "test number", Message = "test message" });

        mockSmsApiService.Setup(x => x.SendSms(It.IsAny<string>(), It.IsAny<string>())).Throws(new ApiException());

        var result = await target.SendSms(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.BadRequest, "ClickSend Api Exception");
    }

    [Fact]
    public async Task SendSms_ShouldReturnBadRequestObjectResult_WhenJSONIsInvalid()
    {
        mockRequest = TestHelpers.CreateMockRequest("{\"mobile\": \"+14055555555\" \"messsage\": \"test message\"}");

        var result = await target.SendSms(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.BadRequest, "Invalid JSON");
    }

    [Fact]
    public async Task SendSms_ShouldReturnInternalServerErrorStatus_WhenOtherExceptionIsThrown()
    {
        mockRequest = TestHelpers.CreateMockRequest(new NotificationData { MobileNumber = "test number", Message = "test message" });
        mockSmsApiService.Setup(x => x.SendSms(It.IsAny<string>(), It.IsAny<string>())).Throws(new FileNotFoundException("Some random exception"));

        var result = await target.SendSms(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.InternalServerError, null);
    }

    [Fact]
    public async Task SendSms_ShouldReturnOkResult_WhenMobileNumberAndMessageAreProvided()
    {
        mockRequest = TestHelpers.CreateMockRequest(new NotificationData { MobileNumber = "test number", Message = "test message" });

        var result = await target.SendSms(mockRequest);

        TestHelpers.ValidateHttpResponseData(result, HttpStatusCode.OK, null);
    }
} 