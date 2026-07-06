using LendAHand.Application.DTOs;
using LendAHand.Application.Interfaces.Services;
using LendAHand.Domain.Entities;
using LendAHand.Domain.Enums;
using LendAHand.Domain.Exceptions;
using LendAHand.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;

        public AuthService(
            ApplicationDbContext context,
            TokenService tokenService,
            IConfiguration config)
        {
            _context = context;
            _tokenService = tokenService;
            _config = config;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email.ToLower());

            if (exists)
                throw new ValidationException("Email already registered");

            if (!Enum.TryParse<UserRole>(dto.Role, out var role))
                throw new ValidationException("Invalid role");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid email or password");

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            // Remember Me
            string? rememberMeToken = null;
            if (dto.RememberMe)
            {
                rememberMeToken = _tokenService.GenerateRememberMeToken();
                user.RememberMeToken = rememberMeToken;
            }

            await _context.SaveChangesAsync();

            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                RememberMeToken = rememberMeToken
            };
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new UnauthorizedException("Invalid or expired refresh token");

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return new AuthResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task LogoutAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
                user.RememberMeToken = null;
                await _context.SaveChangesAsync();
            }
        }
    }
}
