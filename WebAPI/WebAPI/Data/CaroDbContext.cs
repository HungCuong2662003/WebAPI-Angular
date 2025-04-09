using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Data
{
    public class CaroDbContext : IdentityDbContext<User>
    {
        public CaroDbContext(DbContextOptions<CaroDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Rooms> Rooms { get; set; }
        public DbSet<GameMatches> GameMatches { get; set; }
        public DbSet<Moves> Moves { get; set; }
        public DbSet<Ranking> Ranking { get; set; }
        public DbSet<RoomPlayers> RoomPlayers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Moves>()
                .HasOne(m => m.GameMatch)
                .WithMany(g => g.Moves)
                .HasForeignKey(m => m.MatchID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GameMatches>()
                .HasOne(g => g.Player1)
                .WithMany(u => u.MatchesAsPlayer1)
                .HasForeignKey(g => g.Player1ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GameMatches>()
                .HasOne(g => g.Player2)
                .WithMany(u => u.MatchesAsPlayer2)
                .HasForeignKey(g => g.Player2ID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Rooms>()
               .HasIndex(r => r.RoomCode)
               .IsUnique();  // Đảm bảo RoomCode là duy nhất trong cơ sở dữ liệu
                             // Quan hệ một-nhiều: Một phòng có thể có nhiều người chơi

            modelBuilder.Entity<RoomPlayers>()
                .HasOne(rp => rp.Room)   // RoomPlayers có một Room
                .WithMany(r => r.RoomPlayers)  // Room có nhiều RoomPlayers
                .HasForeignKey(rp => rp.RoomId)  // Khóa ngoại là RoomId trong bảng RoomPlayers
                .OnDelete(DeleteBehavior.Restrict);  // Hành vi khi xóa (tuỳ chỉnh)

        }
    }
}
