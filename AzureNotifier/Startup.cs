using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureNotifier;

[ExcludeFromCodeCoverage]
public class Startup
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices(r =>
            {
                // Options
                r.AddOptions<AppSettings>().Configure<IConfiguration>(
                    (settings, configuration) =>
                    {
                        configuration.GetSection("AppSettings").Bind(settings);
                    }
                );

                // Api Wrappers
                r.AddSingleton<IClickSendSmsApiWrapper, ClickSendSmsApiWrapper>();
                r.AddSingleton<IClickSendEmailApiWrapper, ClickSendEmailApiWrapper>();

                // Services
                r.AddSingleton<ISmsApiService, ClickSendSmsApiService>();
                r.AddSingleton<IEmailApiService, ClickSendEmailApiService>();
            }).Build();

        host.Run();
    }
}