using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Data
{
	[Table("Rooms")]
	public class Rooms
	{
		[Key]
		public Guid ID { get; set; }

		[Required, MaxLength(10)]
		public string RoomCode { get; set; } = string.Empty;

		[Required]
		public string OwnerID { get; set; }

		[ForeignKey("OwnerID")]
		public User? Owner { get; set; }

		[MaxLength(50)]
		public string? PasswordRoom { get; set; }

		[Required]
		public bool IsPublic { get; set; } = true;

		[Required, MaxLength(50)]
		public string Status { get; set; } = "Waiting";

		public DateTime CreateAt { get; set; } = DateTime.Now;

		// Navigation property
		public ICollection<GameMatches>? Matches { get; set; }
	}
}
