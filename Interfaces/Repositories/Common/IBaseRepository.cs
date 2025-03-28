﻿using NucpaBalloonsApi.Models.Common;

namespace NucpaBalloonsApi.Interfaces.Repositories.Common
{

    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> InsertAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(string id);
        Task<int> CountAsync();
    }
}
