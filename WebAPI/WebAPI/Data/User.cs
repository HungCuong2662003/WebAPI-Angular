using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Data
{
    public class User : IdentityUser
    {
      public string Firtname { get; set; } = string.Empty;
      public string Lastname { get; set; } = string.Empty;
		public int EloRating { get; set; } = 1000;
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		public ICollection<Rooms>? OwnedRooms { get; set; }
		[InverseProperty("Player1")]
		public ICollection<GameMatches>? MatchesAsPlayer1 { get; set; }
		[InverseProperty("Player2")]
		public ICollection<GameMatches>? MatchesAsPlayer2 { get; set; }
		[InverseProperty("Winner")]
		public ICollection<GameMatches>? MatchesAsWinner { get; set; }
        public ICollection<RoomPlayers>? RoomPlayers { get; set; }
        public Ranking? Ranking { get; set; }
	}

}
