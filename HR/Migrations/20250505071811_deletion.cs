using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Migrations
{
    /// <inheritdoc />
    public partial class deletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_AspNetUsers_EmployeeId",
                table: "Leaves");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Leaves",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Leaves_EmployeeId",
                table: "Leaves",
                newName: "IX_Leaves_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_AspNetUsers_ApplicationUserId",
                table: "Leaves",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_AspNetUsers_ApplicationUserId",
                table: "Leaves");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Leaves",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Leaves_ApplicationUserId",
                table: "Leaves",
                newName: "IX_Leaves_EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_AspNetUsers_EmployeeId",
                table: "Leaves",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
