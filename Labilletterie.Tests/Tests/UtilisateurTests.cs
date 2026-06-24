// Tests/UtilisateurTests.cs

using Xunit;
using Labilletterie.Models;
using Labilletterie.Tests.Helpers;

namespace Labilletterie.Tests.Tests
{
    public class UtilisateurTests
    {
        // ============================================================
        // TEST 1 : Un utilisateur a le rôle "Acheteur" par défaut
        // ============================================================
        [Fact]
        public void Utilisateur_RoleParDefaut_EstAcheteur()
        {
            // ARRANGE : Prépare les données
            var utilisateur = new Utilisateur();

            // ACT : Exécute l'action (ici rien à faire, c'est la valeur par défaut)

            // ASSERT : Vérifie le résultat
            Assert.Equal("Acheteur", utilisateur.Role);
        }

        // ============================================================
        // TEST 2 : Un utilisateur est actif par défaut
        // ============================================================
        [Fact]
        public void Utilisateur_EstActifParDefaut_EstVrai()
        {
            var utilisateur = new Utilisateur();
            Assert.True(utilisateur.EstActif);
        }

        // ============================================================
        // TEST 3 : Un utilisateur a 0 points fidélité par défaut
        // ============================================================
        [Fact]
        public void Utilisateur_PointsFideliteParDefaut_EstZero()
        {
            var utilisateur = new Utilisateur();
            Assert.Equal(0, utilisateur.PointsFidelite);
        }

        // ============================================================
        // TEST 4 : Sauvegarde d'un utilisateur en base
        // ============================================================
        [Fact]
        public async Task Utilisateur_Sauvegarde_EstBienEnBase()
        {
            // ARRANGE
            using var context = DbContextHelper
                .CreerContexteTest("test_utilisateur_sauvegarde");

            var utilisateur = new Utilisateur
            {
                Prenom = "Jean",
                Nom = "Dupont",
                Email = "jean@test.com",
                MotDePasseHash = "hash_test",
                Role = "Acheteur"
            };

            // ACT
            context.Utilisateurs.Add(utilisateur);
            await context.SaveChangesAsync();

            // ASSERT
            var trouve = await context.Utilisateurs.FindAsync(utilisateur.Id);
            Assert.NotNull(trouve);
            Assert.Equal("Jean", trouve.Prenom);
            Assert.Equal("jean@test.com", trouve.Email);
        }

        // ============================================================
        // TEST 5 : Deux utilisateurs avec le même email
        // ============================================================
        [Fact]
        public async Task Utilisateurs_DeuxAvecMemeEmail_SontDifferents()
        {
            // ARRANGE
            using var context = DbContextHelper
                .CreerContexteTest("test_emails_uniques");

            var u1 = new Utilisateur
            {
                Prenom = "Jean",
                Nom = "Dupont",
                Email = "jean@test.com",
                MotDePasseHash = "hash1"
            };

            var u2 = new Utilisateur
            {
                Prenom = "Marie",
                Nom = "Martin",
                Email = "jean@test.com", // Même email
                MotDePasseHash = "hash2"
            };

            context.Utilisateurs.AddRange(u1, u2);
            await context.SaveChangesAsync();

            // ACT
            var utilisateurs = context.Utilisateurs
                .Where(u => u.Email == "jean@test.com")
                .ToList();

            // ASSERT : On trouve bien 2 entrées (la contrainte unique
            // sera gérée au niveau applicatif dans le contrôleur)
            Assert.Equal(2, utilisateurs.Count);
        }
    }
}