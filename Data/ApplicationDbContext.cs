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
        public DbSet<APIRequestSuccessLog> APIRequestSuccessLogs { get; set; } = null!;
        public DbSet<BillAmountParameter> BillAmountParameters { get; set; } = null!;

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

            // Configure APIRequestSuccessLog
            modelBuilder.Entity<APIRequestSuccessLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequestData).IsRequired().HasMaxLength(int.MaxValue);
                entity.Property(e => e.Response).HasMaxLength(int.MaxValue);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Requested).IsRequired();
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.Pmcc).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsBilled).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.BillAmount).HasDefaultValue(10);

                entity.HasIndex(e => e.Requested);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Pmcc);
                entity.HasIndex(e => e.IsBilled);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => new { e.APIRequestId, e.Requested });
                entity.HasIndex(e => new { e.Pmcc, e.IsBilled });
            });

            // Configure BillAmountParameter
            modelBuilder.Entity<BillAmountParameter>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();

                entity.HasIndex(e => e.IsActive);
            });

            // Configure APIRequest
            modelBuilder.Entity<APIRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MethodName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.HospitalId).IsRequired();
                entity.Property(e => e.CipherKey).IsRequired().HasMaxLength(255);
                entity.Property(e => e.HospitalCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasIndex(e => new { e.HospitalId, e.MethodName }).IsUnique();
                entity.HasIndex(e => e.HospitalId);
                entity.HasIndex(e => e.HospitalCode);
            });

            // Seed data for APIRequests
            SeedAPIRequests(modelBuilder);

            // Seed data for BillAmountParameter
            SeedBillAmountParameters(modelBuilder);
        }

        private static void SeedAPIRequests(ModelBuilder modelBuilder)
        {
            // Sample cipher keys and hospital codes for different hospitals
            var hospitalData = new Dictionary<string, (string CipherKey, string HospitalCode)>
            {
                { "H92006568", ("PHilheaLthDuMmy311630", "311630") }, // Default hospital - staging
                { "H12345678", ("YourActualCipherKey123", "311630") }, // Another hospital
                { "H87654321", ("AnotherHospitalKey456", "311630") },  // Another hospital
                // Add more hospitals as needed
            };

            var apiRequests = new List<APIRequest>();
            int currentId = 1;

            // Create API requests for each hospital
            foreach (var hospitalKvp in hospitalData)
            {
                var hospitalId = hospitalKvp.Key;
                var (cipherKey, hospitalCode) = hospitalKvp.Value;

                var hospitalRequests = Enum.GetValues<RequestName>()
                    .Select(requestName => new APIRequest
                    {
                        Id = currentId++, // Sequential unique ID
                        HospitalId = hospitalId,
                        MethodName = requestName.ToString(),
                        CipherKey = cipherKey,
                        HospitalCode = hospitalCode, // Set HospitalCode
                        IsActive = true
                    })
                    .ToArray();

                apiRequests.AddRange(hospitalRequests);
            }

            modelBuilder.Entity<APIRequest>().HasData(apiRequests.ToArray());
        }

        private static void SeedBillAmountParameters(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BillAmountParameter>().HasData(
                new BillAmountParameter
                {
                    Id = 1,
                    Price = 10,
                    IsActive = true
                }
            );
        }
    }
}