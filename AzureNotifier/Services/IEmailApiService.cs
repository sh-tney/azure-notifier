namespace AzureNotifier.Services;

public interface IEmailApiService {
    string SendEmail(string emailAddress, string message, string subject);
} 