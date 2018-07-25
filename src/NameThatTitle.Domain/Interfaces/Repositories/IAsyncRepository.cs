using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NameThatTitle.Domain.Models;

namespace NameThatTitle.Domain.Interfaces.Repositories
{
    public interface IAsyncRepository<TEntity> where TEntity : BaseEntity
    {
        IQueryable<TEntity> All { get; }

        Task<TEntity> GetByIdAsync(int id);

        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
