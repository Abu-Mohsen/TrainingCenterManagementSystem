using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenterSystem.Migrations
{
    /// <inheritdoc />
    public partial class ChangePhoneToPhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Students",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Instructors",
                newName: "PhoneNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Students",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Instructors",
                newName: "Phone");
        }
    }
}
