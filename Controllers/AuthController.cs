using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;
using Ankets.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System;
using System.Linq;

namespace Ankets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Kay�t s�ras�nda bir hata olu�tu" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Giri� s�ras�nda bir hata olu�tu" });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _authService.SendPasswordResetEmailAsync(dto.Email);
                return Ok(new { Message = "�ifre s�f�rlama ba�lant�s� g�nderildi" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Bir hata olu�tu" });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(
                dto.Email,
                dto.Token,
                dto.NewPassword
            );

            return result ?
                Ok(new { Message = "�ifre ba�ar�yla s�f�rland�" }) :
                BadRequest(new { Message = "Ge�ersiz token veya s�resi dolmu�" });
        }

        [HttpPost("logout")]
        public async Task<ActionResult<LogoutResponseDto>> Logout([FromBody] LogoutRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LogoutAsync(dto.RefreshToken, dto.LogoutFromAllDevices);

            return result ?
                Ok(new LogoutResponseDto { Success = true, Message = "Ba�ar�yla ��k�� yap�ld�" }) :
                BadRequest(new LogoutResponseDto { Success = false, Message = "��k�� ba�ar�s�z" });
        }

        [HttpGet("test")]
        [Authorize]
        public IActionResult TestAuth()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            
            return Ok(new {
                UserId = userId,
                Email = email,
                Role = role,
                AllClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            });
        }
    }
}
