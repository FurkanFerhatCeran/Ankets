using Ankets.DTOs.Requests;
using Ankets.DTOs.Responses;
using Ankets.Services;
using Microsoft.AspNetCore.Mvc;

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
                return StatusCode(500, new { Message = "Kayýt sýrasýnda bir hata oluþtu" });
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
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { Message = "Geçersiz kimlik bilgileri" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Giriþ sýrasýnda bir hata oluþtu" });
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
                return Ok(new { Message = "Þifre sýfýrlama baðlantýsý gönderildi" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Bir hata oluþtu" });
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
                Ok(new { Message = "Þifre baþarýyla sýfýrlandý" }) :
                BadRequest(new { Message = "Geçersiz token veya süresi dolmuþ" });
        }

        [HttpPost("logout")]
        public async Task<ActionResult<LogoutResponseDto>> Logout([FromBody] LogoutRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LogoutAsync(dto.RefreshToken, dto.LogoutFromAllDevices);

            return result ?
                Ok(new LogoutResponseDto { Success = true, Message = "Baþarýyla çýkýþ yapýldý" }) :
                BadRequest(new LogoutResponseDto { Success = false, Message = "Çýkýþ baþarýsýz" });
        }
    }
}