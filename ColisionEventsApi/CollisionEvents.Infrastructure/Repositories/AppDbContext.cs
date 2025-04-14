using CollisionEvents.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollisionEvents.Infrastructure.Repositories
{
    public class AppDbContext : DbContext
    {
        public DbSet<CollisionEventMessage> CollisionEventMessages { get; set; }
        public DbSet<SatelliteOperator> SatelliteOperators { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SatelliteOperator>()
                .HasKey(so => so.OperatorId);

            modelBuilder.Entity<CollisionEventMessage>()
                .HasKey(so => so.MessageId);
        }
    }
}