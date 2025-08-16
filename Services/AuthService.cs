using Ankets.Data;
using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;
using Ankets.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Ankets.Services
{
    // Kimlik doðrulama ve yetkilendirme iþ mantýðýný yöneten servis
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerDto)
        {
            // Kullanýcý adý kontrolü
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                throw new ArgumentException("Bu kullanýcý adý zaten alýnmýþ.");

            // Email kontrolü
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                throw new ArgumentException("Bu e-posta adresi zaten kayýtlý.");

            // Þifreyi BCrypt ile hash'le
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Varsayýlan 'user' rolünü bulma
            var defaultRole = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName == "user");
            if (defaultRole == null)
            {
                throw new InvalidOperationException("Varsayýlan 'user' rolü veritabanýnda bulunamadý. Lütfen 'roles' tablosunu kontrol edin.");
            }

            // Yeni kullanýcý oluþtur
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash, // BCrypt ile hashlenmiþ þifre
                NameSurname = registerDto.NameSurname,
                RoleId = defaultRole.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // JWT token oluþtur
            string generatedToken = GenerateJwtToken(user, defaultRole.RoleName);

            return new AuthResponseDto
            {
                Token = generatedToken,
                User = new UserResponseDto
                {
                    UserId = user.UsersId,
                    Username = user.Username,
                    Email = user.Email,
                    NameSurname = user.NameSurname,
                    AvatarUrl = user.AvatarUrl,
                    IsActive = user.IsActive,
                    RoleId = user.RoleId,
                    RoleName = defaultRole.RoleName,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginDto)
        {
            // Kullanýcýyý email ile bul ve rolünü Include ile yükle
            var user = await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == loginDto.Email);

            // Kullanýcý yoksa veya parola yanlýþsa UnauthorizedAccessException fýrlat
            // BCrypt.Net.BCrypt.Verify metodu ile þifreyi kontrol et
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                // Güvenlik nedeniyle, hata mesajýný 'E-posta veya parola hatalý' olarak genel tut
                throw new UnauthorizedAccessException("E-posta veya parola hatalý.");
            }

            // JWT token oluþtur
            string generatedToken = GenerateJwtToken(user, user.Role?.RoleName);

            return new AuthResponseDto
            {
                Token = generatedToken,
                User = new UserResponseDto
                {
                    UserId = user.UsersId,
                    Username = user.Username,
                    Email = user.Email,
                    NameSurname = user.NameSurname,
                    AvatarUrl = user.AvatarUrl,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    RoleId = user.RoleId,
                    RoleName = user.Role?.RoleName
                }
            };
        }

        // JWT token oluþturma metodu
        private string GenerateJwtToken(User user, string roleName)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UsersId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, roleName)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Parola sýfýrlama ve hash'leme metotlarý
        public async Task<bool> LogoutAsync(string? refreshToken, bool logoutFromAllDevices)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }

            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return true;
            }

            var resetToken = GeneratePasswordResetToken();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u =>
                u.Email == email && u.PasswordResetToken == token);

            if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                return false;
            }

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordHash = newPasswordHash;

            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();
            return true;
        }

        private string GeneratePasswordResetToken()
        {
            var tokenBytes = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            return Convert.ToBase64String(tokenBytes);
        }
    }
}
