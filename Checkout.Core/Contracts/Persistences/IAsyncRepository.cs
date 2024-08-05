﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Checkout.Core.Contracts.Persistences
{
	public interface IAsyncRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> GetByIdAsync(Guid id);
		Task<T> AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(Guid id);
	}
}

