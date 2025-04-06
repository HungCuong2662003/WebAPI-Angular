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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<GameMatches>()
				.HasOne<Rooms>()
				.WithMany(r => r.Matches)
				.HasForeignKey(gm => gm.RoomId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<GameMatches>()
				.HasMany(gm => gm.Moves)
				.WithOne(m => m.GameMatch)
				.HasForeignKey(m => m.MatchID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Rooms>()
				.HasOne(r => r.Owner)
				.WithMany()
				.HasForeignKey(r => r.OwnerID)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}

}
