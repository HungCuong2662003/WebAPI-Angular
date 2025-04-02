using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankingsController : ControllerBase
    {
        private readonly CaroDbContext _context;

        public RankingsController(CaroDbContext context)
        {
            _context = context;
        }

        // GET: api/Rankings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ranking>>> GetRanking()
        {
            return await _context.Ranking.ToListAsync();
        }

        // GET: api/Rankings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ranking>> GetRanking(Guid id)
        {
            var ranking = await _context.Ranking.FindAsync(id);

            if (ranking == null)
            {
                return NotFound();
            }

            return ranking;
        }

        // PUT: api/Rankings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRanking(Guid id, Ranking ranking)
        {
            if (id != ranking.ID)
            {
                return BadRequest();
            }

            _context.Entry(ranking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RankingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Rankings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Ranking>> PostRanking(Ranking ranking)
        {
            _context.Ranking.Add(ranking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRanking", new { id = ranking.ID }, ranking);
        }

        // DELETE: api/Rankings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRanking(Guid id)
        {
            var ranking = await _context.Ranking.FindAsync(id);
            if (ranking == null)
            {
                return NotFound();
            }

            _context.Ranking.Remove(ranking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RankingExists(Guid id)
        {
            return _context.Ranking.Any(e => e.ID == id);
        }
    }
}
