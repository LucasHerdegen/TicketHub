using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketHub.API.Migrations
{
    /// <inheritdoc />
    public partial class SoldTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Events",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoldTickets",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SoldTickets",
                table: "Events");
        }
    }
}
