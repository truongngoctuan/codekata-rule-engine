using System;
using System.Threading.Tasks;
using Checkout.Domain.Entities;

namespace Checkout.Core.Contracts.Persistences
{
	public interface IProductRepository : IAsyncRepository<ProductEntity>
	{
		Task<ProductEntity> GetBySkuAsync(string id);
	}
}

