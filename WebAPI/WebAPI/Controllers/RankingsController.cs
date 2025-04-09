//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using WebAPI.Data;

//namespace WebAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RankingsController : ControllerBase
//    {
//        private readonly CaroDbContext _context;

//        public RankingsController(CaroDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/Rankings
//        //[HttpGet]
//        //public async Task<ActionResult<IEnumerable<Ranking>>> GetRanking()
//        //{
//        //    return await _context.Ranking.ToListAsync();
//        //}
//        //lay bxh
//        [HttpGet("ranking")]
//        public async Task<ActionResult<IEnumerable<Ranking>>> GetRanking()
//        {
//            var rankings = await _context.Ranking
//                .OrderByDescending(r => r.EloRating)
//                .Take(10)  // Lấy top 10
//                .ToListAsync();

//            return Ok(rankings);
//        }
//        public void UpdateEloRating(string player1Id, string player2Id, string winnerId)
//        {
//            var player1 = _context.Users.FirstOrDefault(u => u.Id == player1Id);
//            var player2 = _context.Users.FirstOrDefault(u => u.Id == player2Id);

//            if (player1 != null && player2 != null)
//            {
//                // Elo Rating Update Logic
//                const int K = 30;  // Hệ số thay đổi Elo Rating
//                double expectedPlayer1 = 1.0 / (1.0 + Math.Pow(10, (player2.EloRating - player1.EloRating) / 400));
//                double expectedPlayer2 = 1.0 / (1.0 + Math.Pow(10, (player1.EloRating - player2.EloRating) / 400));

//                if (winnerId == player1Id)
//                {
//                    player1.EloRating += (int)(K * (1 - expectedPlayer1));
//                    player2.EloRating += (int)(K * (0 - expectedPlayer2));
//                }
//                else
//                {
//                    player1.EloRating += (int)(K * (0 - expectedPlayer1));
//                    player2.EloRating += (int)(K * (1 - expectedPlayer2));
//                }

//                _context.SaveChanges();
//            }
//        }

//        // GET: api/Rankings/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<Ranking>> GetRanking(Guid id)
//        {
//            var ranking = await _context.Ranking.FindAsync(id);

//            if (ranking == null)
//            {
//                return NotFound();
//            }

//            return ranking;
//        }

//        // PUT: api/Rankings/5
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutRanking(Guid id, Ranking ranking)
//        {
//            if (id != ranking.ID)
//            {
//                return BadRequest();
//            }

//            _context.Entry(ranking).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!RankingExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // POST: api/Rankings
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPost]
//        public async Task<ActionResult<Ranking>> PostRanking(Ranking ranking)
//        {
//            _context.Ranking.Add(ranking);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetRanking", new { id = ranking.ID }, ranking);
//        }

//        // DELETE: api/Rankings/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteRanking(Guid id)
//        {
//            var ranking = await _context.Ranking.FindAsync(id);
//            if (ranking == null)
//            {
//                return NotFound();
//            }

//            _context.Ranking.Remove(ranking);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool RankingExists(Guid id)
//        {
//            return _context.Ranking.Any(e => e.ID == id);
//        }
//    }
//}
