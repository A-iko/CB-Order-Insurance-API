using Microsoft.Extensions.DependencyInjection;

namespace Insurance.Api.BusinessLogic.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            services.AddScoped<InsuranceCalculator>();
            services.AddScoped<SurchargeRuleManager>();
            return services;
        }
    }
}
