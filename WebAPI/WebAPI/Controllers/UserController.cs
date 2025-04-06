using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CaroDbContext _context;

        public UserController(CaroDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUserSortByEloRating()
        {
            // sắp xếp người dùng theo EloRating giảm dần, win, lost, draw
            var users = await _context.Users.OrderByDescending(u => u.EloRating).ThenByDescending(u => u.Win).ThenBy(u => u.Lose).ThenBy(u => u.Draw).ToListAsync();
            return Ok(new
            {
                message = "Lấy danh sách người dùng thành công",
                data = users,
                status = StatusCodes.Status200OK
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUserById(Guid id)
        {
            var user = await _context.Users.FindAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                message = "Lấy thông tin phòng thành công",
                data = user,
                status = StatusCodes.Status200OK
            });
        }
    }
}