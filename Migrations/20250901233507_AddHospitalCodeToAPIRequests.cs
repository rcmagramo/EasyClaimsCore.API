using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyClaimsCore.API.Migrations
{
    /// <inheritdoc />
    public partial class AddHospitalCodeToAPIRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HospitalCode",
                table: "APIRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 1,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 2,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 3,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 4,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 5,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 6,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 7,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 8,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 9,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 10,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 11,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 12,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 13,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 14,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 15,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 16,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 17,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 18,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 19,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 20,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 21,
                column: "HospitalCode",
                value: "311630");

            migrationBuilder.CreateIndex(
                name: "IX_APIRequests_HospitalCode",
                table: "APIRequests",
                column: "HospitalCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_APIRequests_HospitalCode",
                table: "APIRequests");

            migrationBuilder.DropColumn(
                name: "HospitalCode",
                table: "APIRequests");
        }
    }
}
