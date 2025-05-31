using LogTakipAPI.Data;
using LogTakipAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LogTakipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Eğer kullanıcı daha önce kayıt olduysa
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest("Bu email zaten kayıtlı.");
            }

            // Şifreyi hashleyelim (şimdilik düz kayıt, ileride hashing eklersin)
            user.PasswordHash = user.PasswordHash; // hashing yapılabilir
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Kayıt başarılı.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User userData)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userData.Email);
            var durum = "Başarısız";

            if (user == null || user.PasswordHash != userData.PasswordHash)
            {
                await LogGiris(userData.Email, GetIp(), durum);
                return Unauthorized("Geçersiz email veya şifre.");
            }

            durum = "Başarılı";

            // Token oluştur
            var token = GenerateJwtToken(user);
            await LogGiris(user.Email, GetIp(), durum);

            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task LogGiris(string email, string ip, string durum)
        {
            var log = new Log
            {
                Email = email,
                IP = ip,
                Durum = durum,
                Tarih = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        private string GetIp()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "IP alınamadı";
        }
    }
}
