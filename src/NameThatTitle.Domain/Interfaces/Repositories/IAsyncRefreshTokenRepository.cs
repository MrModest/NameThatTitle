using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NameThatTitle.Domain.Models.Token;

namespace NameThatTitle.Domain.Interfaces.Repositories
{
    public interface IAsyncRefreshTokenRepository
    {
        IQueryable<RefreshToken> All { get; }

        Task<RefreshToken> GetByRefreshAsync(string token);
        Task<RefreshToken> GetByAccessAsync(string token);
        Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId);

        Task<RefreshToken> AddAsync(RefreshToken token);
        Task<RefreshToken> UpdateAsync(RefreshToken token);
        Task DeleteAsync(RefreshToken token);
        Task DeleteAsync(IEnumerable<RefreshToken> tokens);
    }
}
