using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Labilletterie.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prenom = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    MotDePasseHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    PointsFidelite = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstActif = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Evenements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titre = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DateEvenement = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Lieu = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Categorie = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NombrePlacesTotal = table.Column<int>(type: "INTEGER", nullable: false),
                    NombrePlacesRestantes = table.Column<int>(type: "INTEGER", nullable: false),
                    PrixBase = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Statut = table.Column<string>(type: "TEXT", nullable: false),
                    EstPrive = table.Column<bool>(type: "INTEGER", nullable: false),
                    MotDePassePrive = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OrganisateurId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evenements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evenements_Utilisateurs_OrganisateurId",
                        column: x => x.OrganisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Billets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CodeQR = table.Column<string>(type: "TEXT", nullable: false),
                    TypeBillet = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PrixPaye = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    StatutScan = table.Column<string>(type: "TEXT", nullable: false),
                    DateAchat = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AcheteurId = table.Column<int>(type: "INTEGER", nullable: false),
                    EvenementId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Billets_Evenements_EvenementId",
                        column: x => x.EvenementId,
                        principalTable: "Evenements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Billets_Utilisateurs_AcheteurId",
                        column: x => x.AcheteurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Billets_AcheteurId",
                table: "Billets",
                column: "AcheteurId");

            migrationBuilder.CreateIndex(
                name: "IX_Billets_EvenementId",
                table: "Billets",
                column: "EvenementId");

            migrationBuilder.CreateIndex(
                name: "IX_Evenements_OrganisateurId",
                table: "Evenements",
                column: "OrganisateurId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Billets");

            migrationBuilder.DropTable(
                name: "Evenements");

            migrationBuilder.DropTable(
                name: "Utilisateurs");
        }
    }
}
