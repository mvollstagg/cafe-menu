using Microsoft.EntityFrameworkCore;

namespace IAndOthers.Core.Data.Services
{
    public class IODbContextSimple : DbContext
    {
        public IODbContextSimple(DbContextOptions<IODbContextSimple> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
