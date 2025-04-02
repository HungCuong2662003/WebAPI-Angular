using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository accountRepo;
        private readonly CaroDbContext _context;

        public AuthController(IAuthRepository repo, CaroDbContext context) {

            accountRepo = repo;
            _context = context;
        }
  

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<User>>> getUser()
        //{
        //    return await _context.Users.ToListAsync();
        //}

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            var result = await accountRepo.SignUpAsync(model);
            return result.Succeeded ? Ok(new { Success = true }) : BadRequest(result.Errors);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest("ID không khớp với user");
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound("Không tìm thấy người dùng");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        // Hàm kiểm tra user tồn tại
        private bool UserExists(string id)
        {
            return _context.Users.Any(u => u.Id == id);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (User == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            var token = await accountRepo.SignInAsync(model);
            return string.IsNullOrEmpty(token)
                ? Unauthorized(new { Message = "Đăng nhập thất bại" })
                : Ok(new { Token = token });
        }


    }
}
