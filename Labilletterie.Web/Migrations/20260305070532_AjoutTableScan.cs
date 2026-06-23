using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Labilletterie.Migrations
{
    /// <inheritdoc />
    public partial class AjoutTableScan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BilletId = table.Column<int>(type: "INTEGER", nullable: false),
                    Resultat = table.Column<string>(type: "TEXT", nullable: false),
                    DateScan = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ScanneParId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scans_Billets_BilletId",
                        column: x => x.BilletId,
                        principalTable: "Billets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scans_Utilisateurs_ScanneParId",
                        column: x => x.ScanneParId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scans_BilletId",
                table: "Scans",
                column: "BilletId");

            migrationBuilder.CreateIndex(
                name: "IX_Scans_ScanneParId",
                table: "Scans",
                column: "ScanneParId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Scans");
        }
    }
}
