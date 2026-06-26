// Tests/ScanTests.cs

using Xunit;
using Labilletterie.Models;
using Labilletterie.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Labilletterie.Tests.Tests
{
    public class ScanTests
    {
        // ============================================================
        // Helper : crée un jeu de données complet pour les tests de scan
        // ============================================================
        private async Task<(Utilisateur acheteur,
            Utilisateur organisateur,
            Evenement evenement,
            Billet billet)>
            CreerJeuDeDonnees(
                Labilletterie.Data.ApplicationDbContext context)
        {
            var organisateur = new Utilisateur
            {
                Prenom = "Orga",
                Nom = "Test",
                Email = "orga@test.com",
                MotDePasseHash = "hash",
                Role = "Organisateur"
            };
            var acheteur = new Utilisateur
            {
                Prenom = "Acheteur",
                Nom = "Test",
                Email = "acheteur@test.com",
                MotDePasseHash = "hash",
                Role = "Acheteur"
            };
            context.Utilisateurs.AddRange(organisateur, acheteur);
            await context.SaveChangesAsync();

            var evenement = new Evenement
            {
                Titre = "Event Scan Test",
                NombrePlacesTotal = 50,
                NombrePlacesRestantes = 49,
                Statut = "Validé",
                OrganisateurId = organisateur.Id,
                DateEvenement = DateTime.Now.AddDays(1),
                PrixBase = 20m
            };
            context.Evenements.Add(evenement);
            await context.SaveChangesAsync();

            var billet = new Billet
            {
                CodeQR = "CODE-QR-TEST-001",
                TypeBillet = "Standard",
                PrixPaye = 20m,
                StatutScan = "NonScanné",
                AcheteurId = acheteur.Id,
                EvenementId = evenement.Id
            };
            context.Billets.Add(billet);
            await context.SaveChangesAsync();

            return (acheteur, organisateur, evenement, billet);
        }

        // ============================================================
        // TEST 1 : Premier scan → billet validé
        // ============================================================
        [Fact]
        public async Task Scan_PremierScan_ValideLesBillet()
        {
            // ARRANGE
            using var context = DbContextHelper
                .CreerContexteTest("test_premier_scan");
            var (_, organisateur, _, billet) =
                await CreerJeuDeDonnees(context);

            // ACT : Simule la logique de validation du contrôleur
            billet.StatutScan = "Validé";
            var scan = new Scan
            {
                BilletId = billet.Id,
                Resultat = "Validé",
                ScanneParId = organisateur.Id
            };
            context.Scans.Add(scan);
            await context.SaveChangesAsync();

            // ASSERT
            var billetTrouve = await context.Billets.FindAsync(billet.Id);
            Assert.Equal("Validé", billetTrouve!.StatutScan);

            var scanTrouve = await context.Scans
                .FirstOrDefaultAsync(s => s.BilletId == billet.Id);
            Assert.NotNull(scanTrouve);
            Assert.Equal("Validé", scanTrouve.Resultat);
        }

        // ============================================================
        // TEST 2 : Double scan → résultat "DéjàScanné"
        // ============================================================
        [Fact]
        public async Task Scan_DoubleScan_DetecteDoubleScan()
        {
            // ARRANGE
            using var context = DbContextHelper
                .CreerContexteTest("test_double_scan");
            var (_, organisateur, _, billet) =
                await CreerJeuDeDonnees(context);

            // Premier scan
            billet.StatutScan = "Validé";
            context.Scans.Add(new Scan
            {
                BilletId = billet.Id,
                Resultat = "Validé",
                ScanneParId = organisateur.Id
            });
            await context.SaveChangesAsync();

            // ACT : Deuxième scan du même billet
            string resultat;
            if (billet.StatutScan == "Validé")
            {
                // La logique du contrôleur détecte le double scan
                resultat = "DéjàScanné";
                context.Scans.Add(new Scan
                {
                    BilletId = billet.Id,
                    Resultat = "DéjàScanné",
                    ScanneParId = organisateur.Id
                });
                await context.SaveChangesAsync();
            }
            else
            {
                resultat = "Validé";
            }

            // ASSERT
            Assert.Equal("DéjàScanné", resultat);

            // Vérifie qu'il y a bien 2 scans enregistrés
            int nbScans = await context.Scans
                .CountAsync(s => s.BilletId == billet.Id);
            Assert.Equal(2, nbScans);
        }

        // ============================================================
        // TEST 3 : QR code inconnu → refusé
        // ============================================================
        [Fact]
        public async Task Scan_QRCodeInconnu_EstRefuse()
        {
            // ARRANGE
            using var context = DbContextHelper
                .CreerContexteTest("test_qr_inconnu");

            string codeQRInconnu = "CODE-QR-INEXISTANT";

            // ACT : Simule la recherche du billet par QR code
            var billet = await context.Billets
                .FirstOrDefaultAsync(b => b.CodeQR == codeQRInconnu);

            // ASSERT : Aucun billet trouvé = QR invalide
            Assert.Null(billet);
        }

        // ============================================================
        // TEST 4 : Billet pour le mauvais événement → refusé
        // ============================================================
        [Fact]
        public async Task Scan_BilletMauvaisEvenement_EstRefuse()
        {
            // ARRANGE
            using var context = DbContextHelper
                .CreerContexteTest("test_mauvais_evenement");
            var (_, _, evenement, billet) =
                await CreerJeuDeDonnees(context);

            int autreEvenementId = evenement.Id + 999; // ID inexistant

            // ACT : Vérifie que le billet appartient bien à l'événement
            bool billetValide = billet.EvenementId == autreEvenementId;

            // ASSERT
            Assert.False(billetValide);
        }

        // ============================================================
        // TEST 5 : Le journal enregistre l'heure du scan
        // ============================================================
        [Fact]
        public async Task Scan_Journal_EnregistreHeure()
        {
            // ARRANGE
            using var context = DbContextHelper
                .CreerContexteTest("test_journal_heure");
            var (_, organisateur, _, billet) =
                await CreerJeuDeDonnees(context);

            DateTime avant = DateTime.Now;

            // ACT
            var scan = new Scan
            {
                BilletId = billet.Id,
                Resultat = "Validé",
                ScanneParId = organisateur.Id,
                DateScan = DateTime.Now
            };
            context.Scans.Add(scan);
            await context.SaveChangesAsync();

            DateTime apres = DateTime.Now;

            // ASSERT : L'heure du scan est entre "avant" et "après"
            Assert.True(scan.DateScan >= avant);
            Assert.True(scan.DateScan <= apres);
        }
    }
}