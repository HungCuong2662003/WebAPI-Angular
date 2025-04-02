using System.Text.RegularExpressions;

namespace WebAPI.Data
{
    public class GameRoom
    {
        public Guid Id { get; set; }
        public string? RoomCode { get; set; }
        public string HostId { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Waiting"; // "Waiting", "Playing", "Ended"

        public User? Host { get; set; }
        public ICollection<GamePlayer>? Players { get; set; }
        public Match? Match { get; set; }
    }
}
