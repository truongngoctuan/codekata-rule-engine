using Microsoft.Extensions.DependencyInjection;

namespace Checkout.RuleEngine.Persistence;

public static class PersistenceRegistration
{
  public static IServiceCollection AddRuleEnginePersistenceService(this IServiceCollection services)
  {
    services.AddScoped<IRuleLoader, RuleJsonFileLoader>();
    return services;
  }
}

