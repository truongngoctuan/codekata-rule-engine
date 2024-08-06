using Checkout.Core.Contracts.Persistences;
using Microsoft.Extensions.DependencyInjection;

namespace Checkout.Persistence;

public static class PersistenceRegistration
{
  public static IServiceCollection AddPersistenceService(this IServiceCollection services)
  {
    services.AddScoped<IProductRepository, ProductRepository>();
    return services;
  }
}

