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
    public class RoomsController : ControllerBase
    {
        private readonly CaroDbContext _context;

        public RoomsController(CaroDbContext context)
        {
            _context = context;
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rooms>>> GetRooms()
        {
            // trả thêm message, totalRows, data, status
            return Ok(new
            {
                message = "Lấy danh sách phòng thành công",
                totalRows = _context.Rooms.Count(),
                data = await _context.Rooms.ToListAsync(),
                status = StatusCodes.Status200OK
            });

        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rooms>> GetRooms(Guid id)
        {
            var rooms = await _context.Rooms.FindAsync(id);

            if (rooms == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                message = "Lấy thông tin phòng thành công",
                data = rooms,
                status = StatusCodes.Status200OK
            });
        }


        [HttpPost("join/{roomCode}")]
        public async Task<ActionResult<Rooms>> JoinRoom(string roomCode, [FromBody] JoinRoomRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy người dùng",
                    status = StatusCodes.Status404NotFound
                });
            }

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode && r.Status == "waiting");

            if (room == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy phòng",
                    status = StatusCodes.Status404NotFound
                });
            }

            if (!room.IsPublic && room.PasswordRoom != request.Password)
                return BadRequest(new { message = "Incorrect password" });

            var match = await _context.GameMatches.FirstOrDefaultAsync(m => m.RoomId == room.ID);

            if (match == null)
            {
                match = new GameMatches
                {
                    RoomId = room.ID,
                    PlayerXID = user.Id.ToString(),
                    PlayerOID = null,
                    CreateAt = DateTime.Now
                };

                _context.GameMatches.Add(match);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Tham gia phòng thành công",
                    data = room,
                    status = StatusCodes.Status200OK
                });
            }


            if (match.PlayerXID != null && match.PlayerOID == null)
            {
                match.PlayerOID = user.Id.ToString();
                room.Status = "playing";
                _context.GameMatches.Update(match);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Tham gia phòng thành công",
                    data = room,
                    status = StatusCodes.Status200OK
                });
            }

            return BadRequest(new
            {
                message = "Phòng đã đầy",
                status = StatusCodes.Status400BadRequest
            });
        }

        // PUT: api/Rooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRooms(Guid id, Rooms rooms)
        {
            if (id != rooms.ID)
            {
                return BadRequest();
            }

            _context.Entry(rooms).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(rooms);
        }

        // POST: api/Rooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
 
 
        [HttpPost]
        public async Task<ActionResult<Rooms>> PostRooms(Rooms rooms)
        {
            var room = new Rooms
            {
                ID = Guid.NewGuid(),
                RoomCode = rooms.RoomCode,
                OwnerID = rooms.OwnerID,
                PasswordRoom = rooms.PasswordRoom,
                IsPublic = rooms.IsPublic,
                Status = rooms.Status,
                CreateAt = DateTime.UtcNow
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRooms), new { id = room.ID }, room);
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRooms(Guid id)
        {
            var rooms = await _context.Rooms.FindAsync(id);
            if (rooms == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(rooms);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomsExists(Guid id)
        {
            return _context.Rooms.Any(e => e.ID == id);
        }
    }
}
