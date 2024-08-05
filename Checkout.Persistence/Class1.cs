using Checkout.Core.Contracts.Persistences;
using Checkout.Domain.Entities;

namespace Checkout.Persistence;

public class ProductRepository : IProductRepository
{
  public Task<ProductEntity> AddAsync(ProductEntity entity)
  {
    throw new NotImplementedException();
  }

  public Task DeleteAsync(Guid id)
  {
    throw new NotImplementedException();
  }

  public Task<IEnumerable<ProductEntity>> GetAllAsync()
  {
    throw new NotImplementedException();
  }

  public Task<ProductEntity> GetByIdAsync(Guid id)
  {
    throw new NotImplementedException();
  }

  public Task<ProductEntity> GetBySkuAsync(string id)
  {
    throw new NotImplementedException();
  }

  public Task UpdateAsync(ProductEntity entity)
  {
    throw new NotImplementedException();
  }
}
