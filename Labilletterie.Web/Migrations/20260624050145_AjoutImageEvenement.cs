using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Labilletterie.Migrations
{
    /// <inheritdoc />
    public partial class AjoutImageEvenement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Evenements",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Evenements");
        }
    }
}
