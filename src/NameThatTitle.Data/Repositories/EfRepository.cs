using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NameThatTitle.Data.DbContexts;
using NameThatTitle.Core.Interfaces.Repositories;
using NameThatTitle.Core.Models;

namespace NameThatTitle.Data.Repositories
{
    public class EfRepository<TEntity> : IRepository<TEntity>, IAsyncRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly ForumContext _context;

        public EfRepository(ForumContext context)
        {
            _context = context;
        }


        public IQueryable<TEntity> All => _context.Set<TEntity>();


        public TEntity GetById(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }


        public TEntity Add(TEntity entity)
        {
            var result = _context.Set<TEntity>().Add(entity).Entity;
            _context.SaveChanges();

            return result;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var result = _context.Set<TEntity>().Add(entity).Entity;
            await _context.SaveChangesAsync();

            return result;
        }


        public TEntity Update(TEntity entity)
        {
            var result = _context.Set<TEntity>().Update(entity).Entity;
            _context.SaveChanges();

            return result;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var result = _context.Set<TEntity>().Update(entity).Entity;
            await _context.SaveChangesAsync();

            return result;
        }


        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
