using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Model;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomPlayersController : ControllerBase
    {
        private readonly CaroDbContext _context;

        public RoomPlayersController(CaroDbContext context)
        {
            _context = context;
        }

        // GET: api/RoomPlayers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomPlayers>>> GetRoomPlayers()
        {
            var players = await _context.RoomPlayers.ToListAsync();
            return Ok(new
            {
                status = 200,
                message = "Successfully retrieved room players",
                data = players
            });
        }
        // GET: api/RoomPlayers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomPlayers>> GetRoomPlayers(Guid id)
        {
            var roomPlayer = await _context.RoomPlayers.FindAsync(id);

            if (roomPlayer == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = "RoomPlayer not found"
                });
            }

            return Ok(new
            {
                status = 200,
                message = "RoomPlayer found",
                data = roomPlayer
            });
        }
        [HttpGet("GetPlayersInRoom/{roomId}")]
        public async Task<IActionResult> GetPlayersInRoom(Guid roomId)
        {
            var players = await _context.RoomPlayers
                .Where(rp => rp.RoomId == roomId)
                .Include(rp => rp.User) // cần include để lấy thông tin người dùng
                .Select(rp => new {
                    userId = rp.UserID,
                    fullName = rp.User.Firstname + " " + rp.User.Lastname
                })
                .ToListAsync();

            return Ok(players);
        }

        // PUT: api/RoomPlayers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomPlayers(Guid id, RoomPlayers roomPlayers)
        {
            if (id != roomPlayers.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomPlayers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomPlayersExists(id))
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

        //// POST: api/RoomPlayers
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<RoomPlayers>> PostRoomPlayers(RoomPlayerModel model)
        //{
        //    // Tạo entity từ DTO
        //    var roomPlayers = new RoomPlayers
        //    {
        //        RoomId = model.RoomId,
        //        UserID = model.UserID
        //    };

        //    _context.RoomPlayers.Add(roomPlayers);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetRoomPlayers", new { id = roomPlayers.Id }, roomPlayers);
        //}


        // DELETE: api/RoomPlayers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomPlayers(Guid id)
        {
            var roomPlayers = await _context.RoomPlayers.FindAsync(id);
            if (roomPlayers == null)
            {
                return NotFound();
            }

            _context.RoomPlayers.Remove(roomPlayers);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomPlayersExists(Guid id)
        {
            return _context.RoomPlayers.Any(e => e.Id == id);
        }


        [HttpPost("join")]
        public async Task<ActionResult<RoomPlayers>> PostRoomPlayers(RoomPlayerModel RoomPlayerModel)
        {
            // Tìm phòng với RoomCode tương ứng
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.ID == RoomPlayerModel.RoomId);

            if (room == null)
            {
                return NotFound("Room not found");
            }

            // Kiểm tra xem người chơi đã trong phòng hay chưa
            bool isPlayerExists = await _context.RoomPlayers
                .AnyAsync(rp => rp.RoomId == RoomPlayerModel.RoomId && rp.UserID == RoomPlayerModel.UserID);

            if (isPlayerExists)
            {
                return BadRequest("Player already exists in this room.");
            }

            // Kiểm tra trạng thái và số lượng của phòng
            if (!room.Status || room.Soluong >= 2)
            {
                return BadRequest("Room is already full or in use.");
            }

            // Tạo entity từ DTO
            var roomPlayers = new RoomPlayers
            {
                RoomId = RoomPlayerModel.RoomId,
                UserID = RoomPlayerModel.UserID
            };
            // Thêm người chơi mới vào bảng RoomPlayers
            _context.RoomPlayers.Add(roomPlayers);
            await _context.SaveChangesAsync();

            // Tăng số lượng người chơi trong phòng
            room.Soluong++;

            // Nếu đủ người thì đặt trạng thái phòng là false (đầy)
            if (room.Soluong >= 2)
            {
                room.Status = false;
            }

            // Cập nhật lại thông tin phòng
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();

         
            return CreatedAtAction(nameof(GetRoomPlayers), new { id = roomPlayers.Id }, new
            {
                Status = 201,
                Message = "Player joined the room successfully.",
                Data = roomPlayers
            });
        }
        [HttpPost("joinpass")]
        public async Task<ActionResult<RoomPlayers>> PostRoomPlayerspass(RoomPlayerModel RoomPlayerModel,  string pass)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.ID == RoomPlayerModel.RoomId && r.PasswordRoom==pass);

            if (room == null)
            {
                return NotFound("Room and pass ");
            }
            bool isPlayerExists = await _context.RoomPlayers
                .AnyAsync(rp => rp.RoomId == RoomPlayerModel.RoomId && rp.UserID == RoomPlayerModel.UserID);

            if (isPlayerExists)
            {
                return BadRequest("Player already exists in this room.");
            }
            if (!room.Status || room.Soluong >= 2)
            {
                return BadRequest("Room is already full or in use.");
            }
            var roomPlayers = new RoomPlayers
            {
                RoomId = RoomPlayerModel.RoomId,
                UserID = RoomPlayerModel.UserID
            };
            _context.RoomPlayers.Add(roomPlayers);
            await _context.SaveChangesAsync();
            room.Soluong++;
            if (room.Soluong >= 2)
            {
                room.Status = false;
            }
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRoomPlayers), new { id = roomPlayers.Id }, new
            {
                Status = 201,
                Message = "Player joined the room successfully.",
                Data = roomPlayers
            });      
        }
     
        [HttpPost("out")]
        public async Task<IActionResult> OutRoom(RoomPlayerModel RoomPlayerModel)
        {
            // Tìm phòng
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.ID == RoomPlayerModel.RoomId);
            if (room == null)
            {
            
                return NotFound(new
                {
                    status = 404,
                    message = "Room not found"
                });
            }

            // Tìm người chơi trong phòng
            var roomPlayer = await _context.RoomPlayers
                .FirstOrDefaultAsync(r => r.RoomId == RoomPlayerModel.RoomId && r.UserID == RoomPlayerModel.UserID);
            if (roomPlayer == null)
            {
                return NotFound("Player not found in this room");
            }

            // Xoá người chơi khỏi phòng
            _context.RoomPlayers.Remove(roomPlayer);
            await _context.SaveChangesAsync();

            // Giảm số lượng người chơi trong phòng
            if (room.Soluong > 0)
            {
                room.Soluong--;
            }

            // Nếu phòng còn trống hoặc chưa đủ người, mở lại phòng
            if (room.Soluong < 2)
            {
                room.Status = true;
            }

            // Cập nhật phòng
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();

            // Trả về phản hồi thành công
            return Ok(new
            {
                status = 200,
                message = "Player removed from room successfully",
                data = roomPlayer
            });
        }

    }
}
