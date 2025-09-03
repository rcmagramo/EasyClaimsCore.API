using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyClaimsCore.API.Migrations
{
    /// <inheritdoc />
    public partial class fixMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 1,
                column: "MethodName",
                value: "GetRestToken");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 2,
                column: "MethodName",
                value: "GetRestMemberPIN");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 3,
                column: "MethodName",
                value: "SearchRestCaseRate");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 4,
                column: "MethodName",
                value: "SearchServerDateTime");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 5,
                column: "MethodName",
                value: "SearchDatabaseDateTime");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 6,
                column: "MethodName",
                value: "GetRestDoctorPAN");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 7,
                column: "MethodName",
                value: "CheckDoctorIfAccredited");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 8,
                column: "MethodName",
                value: "FetchServerVersion");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 9,
                column: "MethodName",
                value: "FindEmployer");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 10,
                column: "MethodName",
                value: "FetchUploadedClaimsMap");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 11,
                column: "MethodName",
                value: "FetchClaimStatus");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 12,
                column: "MethodName",
                value: "isClaimEligibleAPI");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 13,
                column: "MethodName",
                value: "eClaimsFileCheckAPI");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 14,
                column: "MethodName",
                value: "isEsoaValidAPI");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 15,
                column: "MethodName",
                value: "addRequiredApiDocument");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 16,
                column: "MethodName",
                value: "MockResponseAPI");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 17,
                column: "MethodName",
                value: "generatePBEFPDF");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 18,
                column: "MethodName",
                value: "eClaimsApiUpload");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 19,
                column: "MethodName",
                value: "FetchVoucherDetails");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 20,
                column: "MethodName",
                value: "isDRGValidAPI");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CipherKey", "HospitalId", "MethodName", "HospitalCode" },
                values: new object[] { "PHilheaLthDuMmy311630", "H12345678", "MockDecryptResponseAPI", "311630" });

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CipherKey", "HospitalId", "MethodName", "HospitalCode" },
                values: new object[] { "PHilheaLthDuMmy311630", "H12345678", "MockEncryptResponseAPI", "311630" });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                 table: "APIRequests",
                keyColumn: "Id",
                keyValue: 1,
                column: "MethodName",
                value: "GetRestToken");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 2,
                column: "MethodName",
                value: "GetRestMemberPIN");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 3,
                column: "MethodName",
                value: "SearchRestCaseRate");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 4,
                column: "MethodName",
                value: "SearchServerDateTime");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 5,
                column: "MethodName",
                value: "SearchDatabaseDateTime");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 6,
                column: "MethodName",
                value: "GetRestDoctorPAN");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 7,
                column: "MethodName",
                value: "CheckDoctorIfAccredited");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 8,
                column: "MethodName",
                value: "FetchServerVersion");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 9,
                column: "MethodName",
                value: "FindEmployer");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 10,
                column: "MethodName",
                value: "FetchUploadedClaimsMap");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 11,
                column: "MethodName",
                value: "FetchClaimStatus");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 12,
                column: "MethodName",
                value: "isClaimEligibleAPI");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 13,
                column: "MethodName",
                value: "eClaimsFileCheckAPI");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 14,
                column: "MethodName",
                value: "isEsoaValidAPI");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 15,
                column: "MethodName",
                value: "addRequiredApiDocument");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 16,
                column: "MethodName",
                value: "MockResponseAPI");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 17,
                column: "MethodName",
                value: "generatePBEFPDF");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 18,
                column: "MethodName",
                value: "eClaimsApiUpload");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 19,
                column: "MethodName",
                value: "FetchVoucherDetails");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 20,
                column: "MethodName",
                value: "isDRGValidAPI");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 21,
                column: "MethodName",
                value: "MockDecryptResponseAPI");
        }

    }
}
