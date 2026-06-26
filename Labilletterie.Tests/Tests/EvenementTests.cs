// Tests/EvenementTests.cs

using Xunit;
using Labilletterie.Models;
using Labilletterie.Tests.Helpers;

namespace Labilletterie.Tests.Tests
{
    public class EvenementTests
    {
        // ============================================================
        // TEST 1 : Un événement est "EnAttente" par défaut
        // ============================================================
        [Fact]
        public void Evenement_StatutParDefaut_EstEnAttente()
        {
            var evenement = new Evenement();
            Assert.Equal("EnAttente", evenement.Statut);
        }

        // ============================================================
        // TEST 2 : Un événement n'est pas privé par défaut
        // ============================================================
        [Fact]
        public void Evenement_EstPriveParDefaut_EstFaux()
        {
            var evenement = new Evenement();
            Assert.False(evenement.EstPrive);
        }

        // ============================================================
        // TEST 3 : Validation d'un événement
        // ============================================================
        [Fact]
        public async Task Evenement_Validation_ChangeBienLeStatut()
        {
            // ARRANGE
            using var context = DbContextHelper
                .CreerContexteTest("test_validation_evenement");

            var organisateur = new Utilisateur
            {
                Prenom = "Paul",
                Nom = "Orga",
                Email = "paul@orga.com",
                MotDePasseHash = "hash",
                Role = "Organisateur"
            };
            context.Utilisateurs.Add(organisateur);
            await context.SaveChangesAsync();

            var evenement = new Evenement
            {
                Titre = "Concert Test",
                Description = "Un super concert",
                DateEvenement = DateTime.Now.AddDays(30),
                Lieu = "Paris",
                Categorie = "Concert",
                NombrePlacesTotal = 100,
                NombrePlacesRestantes = 100,
                PrixBase = 20m,
                Statut = "EnAttente",
                OrganisateurId = organisateur.Id
            };
            context.Evenements.Add(evenement);
            await context.SaveChangesAsync();

            // ACT : On valide l'événement
            evenement.Statut = "Validé";
            await context.SaveChangesAsync();

            // ASSERT
            var trouve = await context.Evenements.FindAsync(evenement.Id);
            Assert.Equal("Validé", trouve!.Statut);
        }

        // ============================================================
        // TEST 4 : Refus d'un événement
        // ============================================================
        [Fact]
        public async Task Evenement_Refus_ChangeBienLeStatut()
        {
            using var context = DbContextHelper
                .CreerContexteTest("test_refus_evenement");

            var evenement = new Evenement
            {
                Titre = "Event Refusé",
                Statut = "EnAttente",
                OrganisateurId = 1,
                DateEvenement = DateTime.Now.AddDays(10)
            };
            context.Evenements.Add(evenement);
            await context.SaveChangesAsync();

            // ACT
            evenement.Statut = "Refusé";
            await context.SaveChangesAsync();

            // ASSERT
            var trouve = await context.Evenements.FindAsync(evenement.Id);
            Assert.Equal("Refusé", trouve!.Statut);
        }

        // ============================================================
        // TEST 5 : Les places restantes diminuent après un achat
        // ============================================================
        [Fact]
        public async Task Evenement_AchatBillet_DiminueLesPlaces()
        {
            using var context = DbContextHelper
                .CreerContexteTest("test_places_restantes");

            var evenement = new Evenement
            {
                Titre = "Festival Test",
                NombrePlacesTotal = 100,
                NombrePlacesRestantes = 100,
                Statut = "Validé",
                OrganisateurId = 1,
                DateEvenement = DateTime.Now.AddDays(10),
                PrixBase = 15m
            };
            context.Evenements.Add(evenement);
            await context.SaveChangesAsync();

            // ACT : Simule un achat
            evenement.NombrePlacesRestantes--;
            await context.SaveChangesAsync();

            // ASSERT
            var trouve = await context.Evenements.FindAsync(evenement.Id);
            Assert.Equal(99, trouve!.NombrePlacesRestantes);
        }

        // ============================================================
        // TEST 6 : Un événement complet (0 places)
        // ============================================================
        [Fact]
        public void Evenement_SansPlaces_EstComplet()
        {
            var evenement = new Evenement
            {
                NombrePlacesTotal = 100,
                NombrePlacesRestantes = 0
            };

            // Un événement est "complet" si 0 places restantes
            bool estComplet = evenement.NombrePlacesRestantes <= 0;
            Assert.True(estComplet);
        }
    }
}