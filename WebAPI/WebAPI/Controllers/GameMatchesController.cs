using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Model;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameMatchesController : ControllerBase
    {
        private readonly CaroDbContext _context;
        private readonly IHubContext<GameHub> _hubContext;

        public GameMatchesController(CaroDbContext context , IHubContext<GameHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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
        // API để lưu lượt đi của người chơi
        [HttpPost("{matchId}/move")]
        public async Task<IActionResult> MakeMove(Guid matchId, Moves move)
        {
            var match = await _context.GameMatches.FindAsync(matchId);
            if (match == null) return NotFound();

            move.ID = Guid.NewGuid();
            move.MatchID = matchId;
            move.CreateAt = DateTime.Now;

            _context.Moves.Add(move);
            await _context.SaveChangesAsync();

            return Ok(move);
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

        public class StartMatchRequest
        {
            public Guid RoomId { get; set; }
        }

        [HttpPost("StartMatch")]
        public async Task<IActionResult> StartMatch(GameMatcheModel gameMatcheModel)
        {
            var roomId = gameMatcheModel.RoomId;

            var players = await _context.RoomPlayers
                .Where(rp => rp.RoomId == roomId)
                .Select(rp => rp.UserID)
                .ToListAsync();

            if (players.Count < 2)
                return BadRequest("Phòng chưa đủ 2 người để bắt đầu trận đấu.");

            var match = new GameMatches
            {
                ID = Guid.NewGuid(),
                RoomId = roomId,
                Player1ID = players[0],
                Player2ID = players[1],
                NextTurnPlayerID = players[0], // 👈 Bắt đầu từ Player1
                CreateAt = DateTime.Now
            };

            _context.GameMatches.Add(match);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(roomId.ToString())
                .SendAsync("MatchStarted", new
                {
                    matchId = match.ID,
                    player1 = match.Player1ID,
                    player2 = match.Player2ID,
                    nextTurn = match.NextTurnPlayerID
                });

            return Ok(new
            {
                message = "Trận đấu đã bắt đầu.",
                matchId = match.ID,
                player1 = match.Player1ID,
                player2 = match.Player2ID,
                nextTurn = match.NextTurnPlayerID
            });
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
