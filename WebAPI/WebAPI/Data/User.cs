using Microsoft.AspNetCore.Identity;

namespace WebAPI.Data
{
    public class User : IdentityUser
    {
      public string Firtname { get; set; } = string.Empty;
      public string Lastname { get; set; } = string.Empty;
        public int? Rating { get; set; } = 1000;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GamePlayer>? GamePlayers { get; set; }
        public ICollection<GameRoom>? HostedRooms { get; set; }
    }

}
