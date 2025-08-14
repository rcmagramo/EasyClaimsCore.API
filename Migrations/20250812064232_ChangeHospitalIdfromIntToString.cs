using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyClaimsCore.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeHospitalIdfromIntToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HospitalId",
                table: "APIRequests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 1,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 2,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 3,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 4,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 5,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 6,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 7,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 8,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 9,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 10,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 11,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 12,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 13,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 14,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 15,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 16,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 17,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 18,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 19,
                column: "HospitalId",
                value: "H92006568");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 20,
                column: "HospitalId",
                value: "H92006568");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "HospitalId",
                table: "APIRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 1,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 2,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 3,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 4,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 5,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 6,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 7,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 8,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 9,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 10,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 11,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 12,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 13,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 14,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 15,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 16,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 17,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 18,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 19,
                column: "HospitalId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIRequests",
                keyColumn: "Id",
                keyValue: 20,
                column: "HospitalId",
                value: 1);
        }
    }
}
