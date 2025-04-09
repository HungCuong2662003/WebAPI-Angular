using Microsoft.EntityFrameworkCore;
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
		public Guid ID { get; set; } = Guid.NewGuid();

		[Required, MaxLength(10)]
        public string RoomCode { get; set; }

		[Required]
		public string OwnerID { get; set; }

		[ForeignKey("OwnerID")]
		public User? Owner { get; set; }

		[MaxLength(50)]
		public string? PasswordRoom { get; set; }
    
        public int Soluong { get; set; } = 0;  // Gán giá trị mặc định là 0

		public bool IsPublic { get; set; } 

		public bool Status { get; set; } 

		public DateTime CreateAt { get; set; } = DateTime.Now;

		// Navigation property
		public ICollection<GameMatches>? Matches { get; set; }
        public ICollection<RoomPlayers>? RoomPlayers { get; set; }
    }
}
