namespace AzureNotifier.Services;

public interface ISmsApiService {
    string SendSms(string mobileNumber, string message);
} 