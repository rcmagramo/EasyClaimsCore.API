using EasyClaimsCore.API.Data.Entities;
using EasyClaimsCore.API.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace EasyClaimsCore.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<APIRequestLog> APIRequestLogs { get; set; } = null!;
        public DbSet<APIRequest> APIRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure APIRequestLog
            modelBuilder.Entity<APIRequestLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequestData).IsRequired().HasMaxLength(int.MaxValue);
                entity.Property(e => e.Response).HasMaxLength(int.MaxValue);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Requested).IsRequired();
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                entity.HasIndex(e => e.Requested);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.APIRequestId, e.Requested });
            });

            // Configure APIRequest
            modelBuilder.Entity<APIRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MethodName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.HospitalId).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasIndex(e => new { e.HospitalId, e.MethodName }).IsUnique();
            });

            // Seed data for APIRequests
            SeedAPIRequests(modelBuilder);
        }

        private static void SeedAPIRequests(ModelBuilder modelBuilder)
        {
            var apiRequests = Enum.GetValues<RequestName>()
                .Select((requestName, index) => new APIRequest
                {
                    Id = (int)requestName,
                    HospitalId = "H92006568", // Default hospital
                    MethodName = requestName.ToString(),
                    IsActive = true
                })
                .ToArray();

            modelBuilder.Entity<APIRequest>().HasData(apiRequests);
        }
    }
}