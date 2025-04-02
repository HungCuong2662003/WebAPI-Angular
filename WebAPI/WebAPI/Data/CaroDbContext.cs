using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Data
{
    public class CaroDbContext : IdentityDbContext<User>
    {
        public CaroDbContext(DbContextOptions<CaroDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<GameRoom> GameRooms { get; set; }
        public DbSet<GamePlayer> GamePlayers { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GameRoom>()
                .HasOne(r => r.Host)
                .WithMany(u => u.HostedRooms)
                .HasForeignKey(r => r.HostId)
                .OnDelete(DeleteBehavior.Cascade); // vẫn giữ cascade ở đây

            modelBuilder.Entity<GamePlayer>()
                .HasOne(p => p.User)
                .WithMany(u => u.GamePlayers)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict); // hoặc NoAction

            modelBuilder.Entity<GamePlayer>()
                .HasOne(p => p.Room)
                .WithMany(r => r.Players)
                .HasForeignKey(p => p.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Room)
                .WithOne(r => r.Match)
                .HasForeignKey<Match>(m => m.RoomId);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Winner)
                .WithMany()
                .HasForeignKey(m => m.WinnerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
