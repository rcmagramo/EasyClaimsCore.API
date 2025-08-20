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
                entity.Property(e => e.CipherKey).IsRequired().HasMaxLength(255); // Configure CipherKey
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasIndex(e => new { e.HospitalId, e.MethodName }).IsUnique();
                entity.HasIndex(e => e.HospitalId); // Add index for CipherKey lookups
            });

            // Seed data for APIRequests
            SeedAPIRequests(modelBuilder);
        }

        private static void SeedAPIRequests(ModelBuilder modelBuilder)
        {
            // Sample cipher keys for different hospitals
            var hospitalCipherKeys = new Dictionary<string, string>
            {
                { "H92006568", "PHilheaLthDuMmy311630" }, // Default hospital - staging key
                { "H12345678", "YourActualCipherKey123" }, // Another hospital
                { "H87654321", "AnotherHospitalKey456" },  // Another hospital
                // Add more hospitals as needed
            };

            var apiRequests = new List<APIRequest>();
            int currentId = 1;

            // Create API requests for each hospital
            foreach (var hospitalKvp in hospitalCipherKeys)
            {
                var hospitalId = hospitalKvp.Key;
                var hospitalCipherKey = hospitalKvp.Value;

                var hospitalRequests = Enum.GetValues<RequestName>()
                    .Select(requestName => new APIRequest
                    {
                        Id = currentId++, // Sequential unique ID
                        HospitalId = hospitalId,
                        MethodName = requestName.ToString(),
                        CipherKey = hospitalCipherKey,
                        IsActive = true
                    })
                    .ToArray();

                apiRequests.AddRange(hospitalRequests);
            }

            modelBuilder.Entity<APIRequest>().HasData(apiRequests.ToArray());
        }
    }
}