using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MovieMatch.Identity;

[assembly: FunctionsStartup(typeof(MovieMatch.Startup))]
namespace MovieMatch
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<AzureADJwtBearerValidation>();
        }
    }
}