// Services/AuthService.cs
using Ankets.DTOs;
using Ankets.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Ankets.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RegisterResponseDto> Register(RegisterRequestDto registerDto)
        {
            // Kullan�c� ad� kontrol�
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                throw new ArgumentException("Bu kullan�c� ad� zaten al�nm��");

            // Email kontrol�
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                throw new ArgumentException("Bu email zaten kay�tl�");

            // �ifreyi hash'le
            var passwordHash = HashPassword(registerDto.Password);

            // Yeni kullan�c� olu�tur
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                NameSurname = registerDto.NameSurname,
                RoleId = 3, // Default role ID (3)
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new RegisterResponseDto
            {
                UserId = user.UsersId,
                Username = user.Username,
                Email = user.Email,
                NameSurname = user.NameSurname
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}