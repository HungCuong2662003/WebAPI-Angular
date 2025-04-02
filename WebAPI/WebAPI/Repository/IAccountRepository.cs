using Microsoft.AspNetCore.Identity;
using WebAPI.Model;

namespace WebAPI.Repository
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> SignUpAsync(SignUpModel model);
        public Task<string> SignInAsync(SignInModel model);
    }
}
