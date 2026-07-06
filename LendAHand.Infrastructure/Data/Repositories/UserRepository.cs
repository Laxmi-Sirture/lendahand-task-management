using LendAHand.Application.Interfaces.Repositories;
using LendAHand.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Infrastructure.Data.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email.ToLower());

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
            => await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        public async Task<User?> GetByRememberMeTokenAsync(string token)
            => await _context.Users
                .FirstOrDefaultAsync(u => u.RememberMeToken == token);
    }
}
