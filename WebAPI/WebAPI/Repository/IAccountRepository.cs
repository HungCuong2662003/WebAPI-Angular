using Microsoft.AspNetCore.Identity;
using WebAPI.Model;

namespace WebAPI.Repository
{
	public interface IAuthRepository
	{
		public Task<IdentityResult> SignUpAsync(SignUpModel model);
		public Task<string> SignInAsync(SignInModel model);
	}
}
