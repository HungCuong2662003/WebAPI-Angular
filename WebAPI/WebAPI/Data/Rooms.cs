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
		// unique
		[Required, MaxLength(10)]
		public string RoomCode { get; set; } = string.Empty;

		[Required]
		public string OwnerID { get; set; }

		[ForeignKey("OwnerID")]
		public User? Owner { get; set; }

		[MaxLength(6)]
		public string? PasswordRoom { get; set; }

		[Required]
		public bool IsPublic { get; set; } = true;

		// danh sách người chơi trong phòng
		public List<string>? Players { get; set; } = new List<string>();
		// số người chơi tối đa trong phòng
		[Required]
		public int MaxPlayers { get; set; } = 2;

		[Required, MaxLength(50)]
		public string Status { get; set; } = "waiting";

		public DateTime CreateAt { get; set; } = DateTime.Now;

		// Navigation property
		public ICollection<GameMatches>? Matches { get; set; }
	}
}
