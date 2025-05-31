using LogTakipAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogTakipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Sadece admin erişsin
    public class LogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            var logs = await _context.Logs
                .OrderByDescending(l => l.Tarih)
                .ToListAsync();

            return Ok(logs);
        }
    }
}
