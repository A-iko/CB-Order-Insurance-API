using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Insurance.Api.Clients.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddClients(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            services.Configure<ProductApiClientConfiguration>(configurationManager.GetSection("ProductApiClient"));
            services.AddSingleton<ProductApiClient>();
            return services;
        }
    }
}
