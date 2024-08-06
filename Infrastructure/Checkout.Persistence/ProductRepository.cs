using Checkout.Core.Contracts.Persistences;
using Checkout.Domain.Entities;

namespace Checkout.Persistence;

/// <summary>
/// simple implementation to to return product unit price on scanning items
/// </summary>
public class ProductRepository : IProductRepository
{
  Dictionary<string, ProductEntity> products = new Dictionary<string, ProductEntity> {
    {"A", new ProductEntity { SKU = "A", UnitPrice = 50}},
    {"B", new ProductEntity { SKU = "B", UnitPrice = 30}},
    {"C", new ProductEntity { SKU = "C", UnitPrice = 20}},
    {"D", new ProductEntity { SKU = "D", UnitPrice = 15}},
  };
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

  public Task<ProductEntity?> GetBySkuAsync(string sku)
  {
    if (products.ContainsKey(sku)) return Task.FromResult<ProductEntity?>(products[sku]);
    return Task.FromResult<ProductEntity?>(null);
  }

  public Task UpdateAsync(ProductEntity entity)
  {
    throw new NotImplementedException();
  }
}
