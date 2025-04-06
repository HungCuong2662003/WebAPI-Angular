using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Data
{
	public class User : IdentityUser
	{
		public string Firstname { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public int EloRating { get; set; } = 1000;
		public int Win { get; set; } = 0;

		public int Lose { get; set; } = 0;

		public int Draw { get; set; } = 0;

		public DateTime CreatedAt { get; set; } = DateTime.Now;

	}
}
