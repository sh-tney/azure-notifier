using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzureNotifier.Startup))]
namespace AzureNotifier;

[ExcludeFromCodeCoverage]
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        // Services
        builder.Services.AddSingleton<IClickSendSmsApiWrapper, ClickSendSmsApiWrapper>();
        builder.Services.AddSingleton<ISmsApiService, ClickSendSmsApiService>();
    }
}