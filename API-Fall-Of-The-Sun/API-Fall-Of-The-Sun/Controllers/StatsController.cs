using Microsoft.AspNetCore.Mvc;
using API_Fall_Of_The_Sun.Data;
using API_Fall_Of_The_Sun.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API_Fall_Of_The_Sun.Data.Data;

namespace API_Fall_Of_The_Sun.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StatsController(AppDbContext context)
        {
            _context = context;
        }
        // POST api/stats/submit – Dodawanie lub aktualizacja statystyk
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitStats([FromBody] HallOfFame stats)
        {
            if (stats == null)
            {
                return BadRequest("Błąd: Statystyki są puste.");
            }

            if (stats.UserId == 0)
            {
                return BadRequest("Błąd: UserId nie może być zerowy.");
            }

            if (stats.Kills < 0 || stats.Deaths < 0)
            {
                return BadRequest("Błąd: Wartości Kills, Deaths lub TotalScore nie mogą być ujemne.");
            }

            var existingStats = await _context.HallOfFame
                .FirstOrDefaultAsync(s => s.UserId == stats.UserId);

            if (existingStats != null)
            {
                existingStats.Kills += stats.Kills;
                existingStats.Deaths += stats.Deaths;
                existingStats.TotalScore += stats.TotalScore;
            }
            else
            {
                _context.HallOfFame.Add(stats);
            }

            await _context.SaveChangesAsync();
            return Ok("Statystyki zapisane pomyślnie.");
        }
        // Pobieranie wszystkich wyników posortowanych malejąco po TotalScore
        [HttpGet("halloffame")]
        public async Task<IActionResult> GetHallOfFame()
        {
            var results = await _context.HallOfFame
                .Join(_context.Users,
                      hof => hof.UserId,
                      user => user.UserId,
                      (hof, user) => new
                      {
                          user.Username,
                          hof.TotalScore
                      })
                .OrderByDescending(h => h.TotalScore)  // Sortowanie malejąco
                .ToListAsync();

            if (results == null || results.Count == 0)
            {
                return NotFound("Brak wyników w Hall of Fame.");
            }

            return Ok(results);
        }
    }
}
