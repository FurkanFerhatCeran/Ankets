using Ankets.Data;
using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;
using Ankets.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace Ankets.Services
{
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
            // Kullan�c� ad� kontrol�
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                throw new ArgumentException("Bu kullan�c� ad� zaten al�nm��.");

            // Email kontrol�
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                throw new ArgumentException("Bu e-posta adresi zaten kay�tl�.");

            // �ifreyi hash'le
            var passwordHash = HashPassword(registerDto.Password);

            // Varsay�lan 'user' rol�n� bulma
            var defaultRole = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName == "user");
            if (defaultRole == null)
            {
                throw new InvalidOperationException("Varsay�lan 'user' rol� veritaban�nda bulunamad�. L�tfen 'roles' tablosunu kontrol edin.");
            }

            // Yeni kullan�c� olu�tur
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                NameSurname = registerDto.NameSurname,
                RoleId = defaultRole.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // JWT token olu�tur
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
            // Kullan�c�y� sadece email ile bul ve rol�n� Include ile y�kle
            var user = await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("E-posta veya parola hatal�.");
            }

            // JWT token olu�tur
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

        // Bu metot, token'�n i�inde tutulacak bilgileri daha g�venli hale getirir.
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

        // �ifre s�f�rlama ve hash'leme metotlar�
        public async Task<bool> LogoutAsync(string? refreshToken, bool logoutFromAllDevices)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }

            // TODO: Ger�ek veritaban� veya cache operasyonlar�n� buraya ekleyin
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

            var newPasswordHash = HashPassword(newPassword);
            user.PasswordHash = newPasswordHash;

            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();
            return true;
        }

        private string GeneratePasswordResetToken()
        {
            using var rngCsp = new RNGCryptoServiceProvider();
            var randomBytes = new byte[32];
            rngCsp.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var hashedEnteredPassword = HashPassword(enteredPassword);
            return hashedEnteredPassword == storedHash;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}