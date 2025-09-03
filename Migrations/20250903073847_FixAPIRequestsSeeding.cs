using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EasyClaimsCore.API.Migrations
{
    /// <inheritdoc />
    public partial class FixAPIRequestsSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 66);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "APIRequests",
                columns: new[] { "Id", "CipherKey", "HospitalCode", "HospitalId", "IsActive", "MethodName" },
                values: new object[,]
                {
                    { 23, "YourActualCipherKey123", "311630", "H12345678", true, "GetRestToken" },
                    { 24, "YourActualCipherKey123", "311630", "H12345678", true, "GetRestMemberPIN" },
                    { 25, "YourActualCipherKey123", "311630", "H12345678", true, "SearchRestCaseRate" },
                    { 26, "YourActualCipherKey123", "311630", "H12345678", true, "SearchServerDateTime" },
                    { 27, "YourActualCipherKey123", "311630", "H12345678", true, "SearchDatabaseDateTime" },
                    { 28, "YourActualCipherKey123", "311630", "H12345678", true, "GetRestDoctorPAN" },
                    { 29, "YourActualCipherKey123", "311630", "H12345678", true, "CheckDoctorIfAccredited" },
                    { 30, "YourActualCipherKey123", "311630", "H12345678", true, "FetchServerVersion" },
                    { 31, "YourActualCipherKey123", "311630", "H12345678", true, "FindEmployer" },
                    { 32, "YourActualCipherKey123", "311630", "H12345678", true, "FetchUploadedClaimsMap" },
                    { 33, "YourActualCipherKey123", "311630", "H12345678", true, "FetchClaimStatus" },
                    { 34, "YourActualCipherKey123", "311630", "H12345678", true, "isClaimEligibleAPI" },
                    { 35, "YourActualCipherKey123", "311630", "H12345678", true, "eClaimsFileCheckAPI" },
                    { 36, "YourActualCipherKey123", "311630", "H12345678", true, "isEsoaValidAPI" },
                    { 37, "YourActualCipherKey123", "311630", "H12345678", true, "addRequiredApiDocument" },
                    { 38, "YourActualCipherKey123", "311630", "H12345678", true, "MockResponseAPI" },
                    { 39, "YourActualCipherKey123", "311630", "H12345678", true, "generatePBEFPDF" },
                    { 40, "YourActualCipherKey123", "311630", "H12345678", true, "eClaimsApiUpload" },
                    { 41, "YourActualCipherKey123", "311630", "H12345678", true, "FetchVoucherDetails" },
                    { 42, "YourActualCipherKey123", "311630", "H12345678", true, "isDRGValidAPI" },
                    { 43, "YourActualCipherKey123", "311630", "H12345678", true, "MockDecryptResponseAPI" },
                    { 44, "YourActualCipherKey123", "311630", "H12345678", true, "MockEncryptResponseAPI" },
                    { 45, "AnotherHospitalKey456", "311630", "H87654321", true, "GetRestToken" },
                    { 46, "AnotherHospitalKey456", "311630", "H87654321", true, "GetRestMemberPIN" },
                    { 47, "AnotherHospitalKey456", "311630", "H87654321", true, "SearchRestCaseRate" },
                    { 48, "AnotherHospitalKey456", "311630", "H87654321", true, "SearchServerDateTime" },
                    { 49, "AnotherHospitalKey456", "311630", "H87654321", true, "SearchDatabaseDateTime" },
                    { 50, "AnotherHospitalKey456", "311630", "H87654321", true, "GetRestDoctorPAN" },
                    { 51, "AnotherHospitalKey456", "311630", "H87654321", true, "CheckDoctorIfAccredited" },
                    { 52, "AnotherHospitalKey456", "311630", "H87654321", true, "FetchServerVersion" },
                    { 53, "AnotherHospitalKey456", "311630", "H87654321", true, "FindEmployer" },
                    { 54, "AnotherHospitalKey456", "311630", "H87654321", true, "FetchUploadedClaimsMap" },
                    { 55, "AnotherHospitalKey456", "311630", "H87654321", true, "FetchClaimStatus" },
                    { 56, "AnotherHospitalKey456", "311630", "H87654321", true, "isClaimEligibleAPI" },
                    { 57, "AnotherHospitalKey456", "311630", "H87654321", true, "eClaimsFileCheckAPI" },
                    { 58, "AnotherHospitalKey456", "311630", "H87654321", true, "isEsoaValidAPI" },
                    { 59, "AnotherHospitalKey456", "311630", "H87654321", true, "addRequiredApiDocument" },
                    { 60, "AnotherHospitalKey456", "311630", "H87654321", true, "MockResponseAPI" },
                    { 61, "AnotherHospitalKey456", "311630", "H87654321", true, "generatePBEFPDF" },
                    { 62, "AnotherHospitalKey456", "311630", "H87654321", true, "eClaimsApiUpload" },
                    { 63, "AnotherHospitalKey456", "311630", "H87654321", true, "FetchVoucherDetails" },
                    { 64, "AnotherHospitalKey456", "311630", "H87654321", true, "isDRGValidAPI" },
                    { 65, "AnotherHospitalKey456", "311630", "H87654321", true, "MockDecryptResponseAPI" },
                    { 66, "AnotherHospitalKey456", "311630", "H87654321", true, "MockEncryptResponseAPI" }
                });
        }
    }
}
