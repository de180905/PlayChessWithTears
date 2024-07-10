using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineChess.Models.Entities;

namespace OnlineChess.Data
{
    public class OnlineChessDbContext : IdentityDbContext<OnlineChessUser>
    {
        public DbSet<MatchSummary> MatchSummaries { get; set; } // Add this DbSet for MatchSummary

        public OnlineChessDbContext(DbContextOptions<OnlineChessDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.ApplyConfiguration(new OnlineChessUserEntityConfiguration());
            builder.ApplyConfiguration(new MatchSummaryEntityConfiguration()); // Apply configuration for MatchSummary
        }

        public class OnlineChessUserEntityConfiguration : IEntityTypeConfiguration<OnlineChessUser>
        {
            public void Configure(EntityTypeBuilder<OnlineChessUser> builder)
            {
                builder.Property(u => u.FirstName).HasMaxLength(255);
                builder.Property(u => u.LastName).HasMaxLength(255);
                builder.Property(u => u.InGame).HasMaxLength(255);
                builder.HasIndex(u => u.InGame).IsUnique();
            }
        }

        public class MatchSummaryEntityConfiguration : IEntityTypeConfiguration<MatchSummary>
        {
            public void Configure(EntityTypeBuilder<MatchSummary> builder)
            {
                // Set up the primary key
                builder.HasKey(ms => ms.Id);

                // Configure the identity property
                builder.Property(ms => ms.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn();

                // Configure the MatchSummary entity here, including relationships with OnlineChessUser
                builder.HasOne(ms => ms.Black)
                       .WithMany()
                       .HasForeignKey(ms => ms.BlackId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(ms => ms.White)
                       .WithMany()
                       .HasForeignKey(ms => ms.WhiteId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.Restrict);

                builder.Property(ms => ms.Result).HasMaxLength(1);
            }
        }
    }
}
