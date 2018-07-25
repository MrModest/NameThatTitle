using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NameThatTitle.Domain.Models;

namespace NameThatTitle.Domain.Interfaces.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        IQueryable<TEntity> All { get; }

        TEntity GetById(int id);

        TEntity Add(TEntity entity);
        TEntity Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
