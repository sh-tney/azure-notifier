namespace AzureNotifier.Test;

public static class TestHelpers
{
    public static MockHttpRequestData CreateMockRequest(NotificationData body)
    {
        return CreateMockRequest(JsonConvert.SerializeObject(body));
    }

    public static MockHttpRequestData CreateMockRequest(string body)
    {
        var ms = new MemoryStream();
        var sw = new StreamWriter(ms);

        sw.Write(body);
        sw.Flush();
        ms.Position = 0;

        var mockRequest = new MockHttpRequestData(new Mock<FunctionContext>().Object, new Uri("http://google.com"), sw.BaseStream);

        return mockRequest;
    }

    public static void ValidateHttpResponseData(HttpResponseData result, HttpStatusCode code, string? bodyText = null)
    {
        Assert.Equal(code, result.StatusCode);
        if (bodyText != null)
        {
            (result.Body as MemoryStream).Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(result.Body);
            string resultData = reader.ReadToEnd();
            Assert.Equal(bodyText, resultData);
        }
    }
}