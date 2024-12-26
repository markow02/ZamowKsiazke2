using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZamowKsiazke.Migrations
{
    /// <inheritdoc />
    public partial class AddBookBorrowingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BorrowingDays",
                table: "BookBorrowings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BorrowingPrice",
                table: "BookBorrowings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ContactPreference",
                table: "BookBorrowings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BookBorrowings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BorrowingPrice",
                table: "Book",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableForBorrowing",
                table: "Book",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxBorrowingDays",
                table: "Book",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowingDays",
                table: "BookBorrowings");

            migrationBuilder.DropColumn(
                name: "BorrowingPrice",
                table: "BookBorrowings");

            migrationBuilder.DropColumn(
                name: "ContactPreference",
                table: "BookBorrowings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BookBorrowings");

            migrationBuilder.DropColumn(
                name: "BorrowingPrice",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "IsAvailableForBorrowing",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "MaxBorrowingDays",
                table: "Book");
        }
    }
}
