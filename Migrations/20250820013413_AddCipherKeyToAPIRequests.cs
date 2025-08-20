using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EasyClaimsCore.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCipherKeyToAPIRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CipherKey",
                table: "APIRequests",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 1,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 2,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 3,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 4,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 5,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 6,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 7,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 8,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 9,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 10,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 11,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 12,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 13,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 14,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 15,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 16,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 17,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 18,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 19,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 20,
                column: "CipherKey",
                value: "PHilheaLthDuMmy311630");

            migrationBuilder.UpdateData(
              table: "APIRequests",
              keyColumn: "Id",
              keyValue: 21,
              column: "CipherKey",
              value: "PHilheaLthDuMmy311630");

            migrationBuilder.CreateIndex(
                name: "IX_APIRequests_HospitalId",
                table: "APIRequests",
                column: "HospitalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_APIRequests_HospitalId",
                table: "APIRequests");

            migrationBuilder.DropColumn(
                name: "CipherKey",
                table: "APIRequests");
        }
    }
}
