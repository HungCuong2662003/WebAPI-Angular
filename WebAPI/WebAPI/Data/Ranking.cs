using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Data
{
	[Table("Ranking")]
	public class Ranking
	{
		[Key]
		public Guid ID { get; set; }

		[Required]
		public string UserID { get; set; }

		[ForeignKey("UserID")]
		public User? User { get; set; }

		[Required]
		public int Win { get; set; }

		[Required]
		public int Lose { get; set; }

		[Required]
		public int Draw { get; set; }

		[Required]
		public int EloRating { get; set; }
	}
}
