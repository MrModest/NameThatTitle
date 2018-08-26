using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NameThatTitle.Data.DbContexts;
using NameThatTitle.Domain.Interfaces.Repositories;
using NameThatTitle.Domain.Models.Token;

namespace NameThatTitle.Data.Repositories
{
    public class RefreshTokenRepository : IAsyncRefreshTokenRepository
    {
        private readonly AppIdentityContext _context;

        public RefreshTokenRepository(AppIdentityContext context)
        {
            _context = context;
        }

        public IQueryable<RefreshToken> All => _context.RefreshTokens;


        public async Task<RefreshToken> GetByAccessAsync(string token)
        {
            return await _context.RefreshTokens.Include(rt => rt.UserAccount).FirstOrDefaultAsync(t => t.Access == token);
        }

        public async Task<RefreshToken> GetByRefreshAsync(string token)
        {
            return await _context.RefreshTokens.Include(rt => rt.UserAccount).FirstOrDefaultAsync(t => t.Refresh == token);
        }

        public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId)
        {
            return await _context.RefreshTokens.Where(t => t.UserId == userId).Include(rt => rt.UserAccount).ToListAsync();
        }


        public async Task<RefreshToken> AddAsync(RefreshToken token)
        {
            var result = _context.RefreshTokens.Add(token).Entity;
            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<RefreshToken> UpdateAsync(RefreshToken token)
        {
            var result = _context.RefreshTokens.Update(token).Entity;
            await _context.SaveChangesAsync();

            return result;
        }

        public async Task DeleteAsync(RefreshToken token)
        {
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(IEnumerable<RefreshToken> tokens)
        {
            _context.RefreshTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();
        }
    }
}
