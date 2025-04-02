namespace WebAPI.Data
{
    public class GamePlayer
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string? Symbol { get; set; }
        public bool IsHost { get; set; }

        public GameRoom? Room { get; set; }
        public User? User { get; set; }
    }

}
