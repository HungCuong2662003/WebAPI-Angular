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
    public class GamePlayersController : ControllerBase
    {
        private readonly CaroDbContext _context;

        public GamePlayersController(CaroDbContext context)
        {
            _context = context;
        }

        // GET: api/GamePlayers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GamePlayer>>> GetGamePlayers()
        {
            return await _context.GamePlayers.ToListAsync();
        }

        // GET: api/GamePlayers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GamePlayer>> GetGamePlayer(Guid id)
        {
            var gamePlayer = await _context.GamePlayers.FindAsync(id);

            if (gamePlayer == null)
            {
                return NotFound();
            }

            return gamePlayer;
        }

        // PUT: api/GamePlayers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGamePlayer(Guid id, GamePlayer gamePlayer)
        {
            if (id != gamePlayer.Id)
            {
                return BadRequest();
            }

            _context.Entry(gamePlayer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GamePlayerExists(id))
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

        // POST: api/GamePlayers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GamePlayer>> PostGamePlayer(GamePlayer gamePlayer)
        {
            _context.GamePlayers.Add(gamePlayer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGamePlayer", new { id = gamePlayer.Id }, gamePlayer);
        }

        // DELETE: api/GamePlayers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGamePlayer(Guid id)
        {
            var gamePlayer = await _context.GamePlayers.FindAsync(id);
            if (gamePlayer == null)
            {
                return NotFound();
            }

            _context.GamePlayers.Remove(gamePlayer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GamePlayerExists(Guid id)
        {
            return _context.GamePlayers.Any(e => e.Id == id);
        }
    }
}
