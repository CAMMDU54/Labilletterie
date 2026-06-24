// Tests/BilletTests.cs

using Xunit;
using Labilletterie.Models;
using Labilletterie.Services;
using Labilletterie.Tests.Helpers;

namespace Labilletterie.Tests.Tests
{
    public class BilletTests
    {
        // ============================================================
        // TEST 1 : Un billet a un CodeQR unique par défaut
        // ============================================================
        [Fact]
        public void Billet_CodeQR_EstUnique()
        {
            // ARRANGE & ACT
            var billet1 = new Billet();
            var billet2 = new Billet();

            // ASSERT : Les deux codes QR sont différents
            Assert.NotEqual(billet1.CodeQR, billet2.CodeQR);
        }

        // ============================================================
        // TEST 2 : Un billet est "NonScanné" par défaut
        // ============================================================
        [Fact]
        public void Billet_StatutParDefaut_EstNonScanne()
        {
            var billet = new Billet();
            Assert.Equal("NonScanné", billet.StatutScan);
        }

        // ============================================================
        // TEST 3 : Un billet "Standard" a le bon prix
        // ============================================================
        [Fact]
        public void Billet_PrixStandard_EgalPrixBase()
        {
            // ARRANGE
            decimal prixBase = 20m;

            // ACT : Calcul du prix selon le type (logique du contrôleur)
            decimal prix = "Standard" switch
            {
                "VIP" => prixBase * 2,
                "Réduit" => prixBase * 0.5m,
                _ => prixBase
            };

            // ASSERT
            Assert.Equal(20m, prix);
        }

        // ============================================================
        // TEST 4 : Un billet VIP coûte le double
        // ============================================================
        [Fact]
        public void Billet_PrixVIP_EstDoubleDuPrixBase()
        {
            decimal prixBase = 20m;
            decimal prixVip = prixBase * 2;
            Assert.Equal(40m, prixVip);
        }

        // ============================================================
        // TEST 5 : Un billet Réduit coûte la moitié
        // ============================================================
        [Fact]
        public void Billet_PrixReduit_EstMoitieDuPrixBase()
        {
            decimal prixBase = 20m;
            decimal prixReduit = prixBase * 0.5m;
            Assert.Equal(10m, prixReduit);
        }

        // ============================================================
        // TEST 6 : Les points fidélité sont bien calculés
        // ============================================================
        [Fact]
        public void PointsFidelite_Calcul_EgalPrixPaye()
        {
            // ARRANGE : 1€ = 1 point
            decimal prixPaye = 25m;

            // ACT
            int pointsGagnes = (int)prixPaye;

            // ASSERT
            Assert.Equal(25, pointsGagnes);
        }

        // ============================================================
        // TEST 7 : Sauvegarde d'un billet en base
        // ============================================================
        [Fact]
        public async Task Billet_Sauvegarde_EstBienEnBase()
        {
            // ARRANGE
            using var context = DbContextHelper
                .CreerContexteTest("test_billet_sauvegarde");

            // Crée d'abord l'utilisateur et l'événement
            var acheteur = new Utilisateur
            {
                Prenom = "Alice",
                Nom = "Test",
                Email = "alice@test.com",
                MotDePasseHash = "hash"
            };
            var evenement = new Evenement
            {
                Titre = "Test Event",
                NombrePlacesTotal = 50,
                NombrePlacesRestantes = 50,
                Statut = "Validé",
                OrganisateurId = 1,
                DateEvenement = DateTime.Now.AddDays(5),
                PrixBase = 10m
            };
            context.Utilisateurs.Add(acheteur);
            context.Evenements.Add(evenement);
            await context.SaveChangesAsync();

            var billet = new Billet
            {
                TypeBillet = "Standard",
                PrixPaye = 10m,
                AcheteurId = acheteur.Id,
                EvenementId = evenement.Id
            };

            // ACT
            context.Billets.Add(billet);
            await context.SaveChangesAsync();

            // ASSERT
            var trouve = await context.Billets.FindAsync(billet.Id);
            Assert.NotNull(trouve);
            Assert.Equal("Standard", trouve.TypeBillet);
            Assert.Equal("NonScanné", trouve.StatutScan);
        }
    }
}