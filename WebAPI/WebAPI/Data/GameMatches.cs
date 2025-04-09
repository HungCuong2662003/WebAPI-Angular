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

		[ForeignKey("RoomId")]
		public Rooms? Room { get; set; }

		[Required]
		public string Player1ID { get; set; }

		[ForeignKey("Player1ID")]
		[InverseProperty("MatchesAsPlayer1")]
		public User? Player1 { get; set; }

		[Required]
		public string Player2ID { get; set; }

		[ForeignKey("Player2ID")]
		[InverseProperty("MatchesAsPlayer2")]
		public User? Player2 { get; set; }
        public string? NextTurnPlayerID { get; set; }
        // WinnerID có thể null khi trận đấu chưa có người chiến thắng.
        public string? WinnerID { get; set; }

		[ForeignKey("WinnerID")]
		[InverseProperty("MatchesAsWinner")]
		public User? Winner { get; set; }

		public DateTime CreateAt { get; set; } = DateTime.Now;

		// Navigation property
		public ICollection<Moves>? Moves { get; set; }
	}
}
