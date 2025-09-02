using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyClaimsCore.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAPIRequestSuccessLogAndBillAmountParameter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APIRequestSuccessLogs",
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
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Pmcc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsBilled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    BillAmount = table.Column<int>(type: "int", nullable: false, defaultValue: 10)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIRequestSuccessLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APIRequestSuccessLogs_APIRequests_APIRequestId",
                        column: x => x.APIRequestId,
                        principalTable: "APIRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillAmountParameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillAmountParameters", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BillAmountParameters",
                columns: new[] { "Id", "IsActive", "Price" },
                values: new object[] { 1, true, 10 });

            migrationBuilder.CreateIndex(
                name: "IX_APIRequestSuccessLogs_APIRequestId_Requested",
                table: "APIRequestSuccessLogs",
                columns: new[] { "APIRequestId", "Requested" });

            migrationBuilder.CreateIndex(
                name: "IX_APIRequestSuccessLogs_IsActive",
                table: "APIRequestSuccessLogs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_APIRequestSuccessLogs_IsBilled",
                table: "APIRequestSuccessLogs",
                column: "IsBilled");

            migrationBuilder.CreateIndex(
                name: "IX_APIRequestSuccessLogs_Pmcc",
                table: "APIRequestSuccessLogs",
                column: "Pmcc");

            migrationBuilder.CreateIndex(
                name: "IX_APIRequestSuccessLogs_Pmcc_IsBilled",
                table: "APIRequestSuccessLogs",
                columns: new[] { "Pmcc", "IsBilled" });

            migrationBuilder.CreateIndex(
                name: "IX_APIRequestSuccessLogs_Requested",
                table: "APIRequestSuccessLogs",
                column: "Requested");

            migrationBuilder.CreateIndex(
                name: "IX_APIRequestSuccessLogs_Status",
                table: "APIRequestSuccessLogs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BillAmountParameters_IsActive",
                table: "BillAmountParameters",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIRequestSuccessLogs");

            migrationBuilder.DropTable(
                name: "BillAmountParameters");
        }
    }
}
