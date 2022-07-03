using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(AzureNotifier.Startup))]
namespace AzureNotifier;

[ExcludeFromCodeCoverage]
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        // Options
        builder.Services.AddOptions<AppSettings>().Configure<IConfiguration>(
            (settings, configuration) =>
            {
                configuration.GetSection("AppSettings").Bind(settings);
            }
        );

        // Api Wrappers
        builder.Services.AddSingleton<IClickSendSmsApiWrapper, ClickSendSmsApiWrapper>();
        builder.Services.AddSingleton<IClickSendEmailApiWrapper, ClickSendEmailApiWrapper>();

        // Services
        builder.Services.AddSingleton<ISmsApiService, ClickSendSmsApiService>();
        builder.Services.AddSingleton<IEmailApiService, ClickSendEmailApiService>();
    }
}