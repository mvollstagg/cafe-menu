using IAndOthers.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IAndOthers.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<
            ApplicationUser, ApplicationRole, long, 
            IdentityUserClaim<long>, ApplicationUserRole, 
            IdentityUserLogin<long>, IdentityRoleClaim<long>, 
            IdentityUserToken<long>>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<MediaItem> MediaItems { get; set; }
        public DbSet<AuthGuestUser> AuthGuestUsers { get; set; }
        public DbSet<AuthRefreshToken> AuthRefreshTokens { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<ProductProperty> ProductProperties { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Product-Category relationship
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductProperty relationships
            builder.Entity<ProductProperty>()
                .HasOne(pp => pp.Product)
                .WithMany(p => p.ProductProperties)
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductProperty>()
                .HasOne(pp => pp.Property)
                .WithMany(p => p.ProductProperties)
                .HasForeignKey(pp => pp.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
