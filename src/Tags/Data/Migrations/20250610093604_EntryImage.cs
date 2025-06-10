using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TagStudio.Tags.Data.Migrations
{
    /// <inheritdoc />
    public partial class EntryImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Entries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Entries");
        }
    }
}
