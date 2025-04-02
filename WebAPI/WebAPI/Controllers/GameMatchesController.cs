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
    public class GameMatchesController : ControllerBase
    {
        private readonly CaroDbContext _context;

        public GameMatchesController(CaroDbContext context)
        {
            _context = context;
        }

        // GET: api/GameMatches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameMatches>>> GetGameMatches()
        {
            return await _context.GameMatches.ToListAsync();
        }

        // GET: api/GameMatches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameMatches>> GetGameMatches(Guid id)
        {
            var gameMatches = await _context.GameMatches.FindAsync(id);

            if (gameMatches == null)
            {
                return NotFound();
            }

            return gameMatches;
        }

        // PUT: api/GameMatches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGameMatches(Guid id, GameMatches gameMatches)
        {
            if (id != gameMatches.ID)
            {
                return BadRequest();
            }

            _context.Entry(gameMatches).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameMatchesExists(id))
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

        // POST: api/GameMatches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GameMatches>> PostGameMatches(GameMatches gameMatches)
        {
            _context.GameMatches.Add(gameMatches);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGameMatches", new { id = gameMatches.ID }, gameMatches);
        }

        // DELETE: api/GameMatches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGameMatches(Guid id)
        {
            var gameMatches = await _context.GameMatches.FindAsync(id);
            if (gameMatches == null)
            {
                return NotFound();
            }

            _context.GameMatches.Remove(gameMatches);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameMatchesExists(Guid id)
        {
            return _context.GameMatches.Any(e => e.ID == id);
        }
    }
}
