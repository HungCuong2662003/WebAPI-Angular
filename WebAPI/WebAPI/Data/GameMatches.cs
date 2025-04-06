using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Data
{
	[Table("GameMatches")]
	public class GameMatches
	{
		[Key]
		public Guid ID { get; set; }

		[Required]
		public Guid RoomId { get; set; }

		[Required]
		public string PlayerXID { get; set; }

		[Required]
		public string PlayerOID { get; set; }

		public string? WinnerID { get; set; }

		public List<Moves>? Moves { get; set; } = new List<Moves>();
		public DateTime CreateAt { get; set; } = DateTime.Now;

		// Navigation property
	}
}
