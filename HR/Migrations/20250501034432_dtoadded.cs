using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class dtoadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeavePoints",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "PaidLeaveDays",
                table: "Leaves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnpaidLeaveDays",
                table: "Leaves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LeavePoints",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidLeaveDays",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "UnpaidLeaveDays",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "LeavePoints",
                table: "Employees");

            migrationBuilder.AddColumn<int>(
                name: "LeavePoints",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
