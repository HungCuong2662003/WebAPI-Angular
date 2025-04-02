namespace WebAPI.Data
{
    public class Match
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }

        public string? WinnerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Duration { get; set; } // tính bằng giây

        public GameRoom? Room { get; set; }
        public User? Winner { get; set; }
    }
}
