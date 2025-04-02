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
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository accountRepo;
        private readonly CaroDbContext _context;

        public AccountController(IAccountRepository repo, CaroDbContext context) {

            accountRepo = repo;
            _context = context;
        }
  

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> getUser()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            var result = await accountRepo.SignUpAsync(model);
            return result.Succeeded ? Ok(new { Success = true }) : BadRequest(result.Errors);
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
