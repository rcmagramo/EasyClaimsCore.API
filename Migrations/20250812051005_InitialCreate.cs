using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EasyClaimsCore.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APIRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HospitalId = table.Column<int>(type: "int", nullable: false),
                    MethodName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APIRequestLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    APIRequestId = table.Column<int>(type: "int", nullable: false),
                    ChargeableItems = table.Column<int>(type: "int", nullable: false),
                    RequestData = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Requested = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Responded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIRequestLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APIRequestLogs_APIRequests_APIRequestId",
                        column: x => x.APIRequestId,
                        principalTable: "APIRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "APIRequests",
                columns: new[] { "Id", "HospitalId", "IsActive", "MethodName" },
                values: new object[,]
                {
                    { 1, 1, true, "GetRestToken" },
                    { 2, 1, true, "GetRestMemberPIN" },
                    { 3, 1, true, "SearchRestCaseRate" },
                    { 4, 1, true, "SearchServerDateTime" },
                    { 5, 1, true, "SearchDatabaseDateTime" },
                    { 6, 1, true, "GetRestDoctorPAN" },
                    { 7, 1, true, "CheckDoctorIfAccredited" },
                    { 8, 1, true, "FetchServerVersion" },
                    { 9, 1, true, "FindEmployer" },
                    { 10, 1, true, "FetchUploadedClaimsMap" },
                    { 11, 1, true, "FetchClaimStatus" },
                    { 12, 1, true, "isClaimEligibleAPI" },
                    { 13, 1, true, "eClaimsFileCheckAPI" },
                    { 14, 1, true, "isEsoaValidAPI" },
                    { 15, 1, true, "addRequiredApiDocument" },
                    { 16, 1, true, "MockResponseAPI" },
                    { 17, 1, true, "generatePBEFPDF" },
                    { 18, 1, true, "eClaimsApiUpload" },
                    { 19, 1, true, "FetchVoucherDetails" },
                    { 20, 1, true, "isDRGValidAPI" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_APIRequestLogs_APIRequestId_Requested",
                table: "APIRequestLogs",
                columns: new[] { "APIRequestId", "Requested" });

            migrationBuilder.CreateIndex(
                name: "IX_APIRequestLogs_Requested",
                table: "APIRequestLogs",
                column: "Requested");

            migrationBuilder.CreateIndex(
                name: "IX_APIRequestLogs_Status",
                table: "APIRequestLogs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_APIRequests_HospitalId_MethodName",
                table: "APIRequests",
                columns: new[] { "HospitalId", "MethodName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIRequestLogs");

            migrationBuilder.DropTable(
                name: "APIRequests");
        }
    }
}
