// Data/SeedData.cs

using Labilletterie.Models;
using Microsoft.EntityFrameworkCore;

namespace Labilletterie.Data
{
    public static class SeedData
    {
        public static async Task InitialiserAsync(ApplicationDbContext context)
        {
            // ============================================================
            // RÉINITIALISATION COMPLÈTE
            // ============================================================

            // Supprime toutes les données dans le bon ordre
            // (respect des clés étrangères)
            context.Scans.RemoveRange(context.Scans);
            context.Billets.RemoveRange(context.Billets);
            context.Evenements.RemoveRange(context.Evenements);
            context.Utilisateurs.RemoveRange(context.Utilisateurs);
            await context.SaveChangesAsync();

            // Réinitialise les auto-incréments SQLite
            await context.Database.ExecuteSqlRawAsync(
                "DELETE FROM sqlite_sequence WHERE name IN " +
                "('Scans','Billets','Evenements','Utilisateurs')");

            await context.SaveChangesAsync();

            // ============================================================
            // UTILISATEURS
            // ============================================================

            // Super Admin
            var superAdmin = new Utilisateur
            {
                Prenom = "Super",
                Nom = "Admin",
                Email = "superadmin@labilletterie.fr",
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("Admin1234!"),
                Role = "SuperAdmin",
                PointsFidelite = 0,
                EstActif = true
            };

            // Admin
            var admin = new Utilisateur
            {
                Prenom = "Marie",
                Nom = "Dupont",
                Email = "admin@labilletterie.fr",
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("Admin1234!"),
                Role = "Admin",
                PointsFidelite = 0,
                EstActif = true
            };

            // Organisateurs
            var orga1 = new Utilisateur
            {
                Prenom = "Lucas",
                Nom = "Martin",
                Email = "lucas.martin@orga.fr",
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("Orga1234!"),
                Role = "Organisateur",
                PointsFidelite = 0,
                EstActif = true
            };

            var orga2 = new Utilisateur
            {
                Prenom = "Sophie",
                Nom = "Bernard",
                Email = "sophie.bernard@orga.fr",
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("Orga1234!"),
                Role = "Organisateur",
                PointsFidelite = 0,
                EstActif = true
            };

            var orga3 = new Utilisateur
            {
                Prenom = "Thomas",
                Nom = "Leclerc",
                Email = "thomas.leclerc@orga.fr",
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("Orga1234!"),
                Role = "Organisateur",
                PointsFidelite = 0,
                EstActif = true
            };

            // Acheteurs
            var acheteur1 = new Utilisateur
            {
                Prenom = "Alice",
                Nom = "Moreau",
                Email = "alice@acheteur.fr",
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("User1234!"),
                Role = "Acheteur",
                PointsFidelite = 45,
                EstActif = true
            };

            var acheteur2 = new Utilisateur
            {
                Prenom = "Paul",
                Nom = "Lefebvre",
                Email = "paul@acheteur.fr",
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("User1234!"),
                Role = "Acheteur",
                PointsFidelite = 120,
                EstActif = true
            };

            context.Utilisateurs.AddRange(
                superAdmin, admin,
                orga1, orga2, orga3,
                acheteur1, acheteur2
            );
            await context.SaveChangesAsync();

            // ============================================================
            // ÉVÉNEMENTS — CONCERTS (3)
            // ============================================================

            var concerts = new List<Evenement>
            {
                new Evenement
                {
                    Titre                 = "Nuit Électro — Paris",
                    Description           = "Une nuit electro inoubliable avec les meilleurs DJs de la scène parisienne. Sound system XXL, light show immersif et ambiance garantie jusqu'au bout de la nuit. Portes ouvertes à 22h, premier DJ set à 23h.",
                    DateEvenement         = DateTime.Now.AddDays(15),
                    Lieu                  = "Warehouse Paris, 42 rue de la Roquette, 75011 Paris",
                    Categorie             = "Concert",
                    NombrePlacesTotal     = 500,
                    NombrePlacesRestantes = 342,
                    PrixBase              = 18.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1470229722913-7c0e2dbbafd3?w=800",
                    OrganisateurId        = orga1.Id
                },
                new Evenement
                {
                    Titre                 = "Jazz au Parc — Lyon",
                    Description           = "Festival de jazz en plein air dans le magnifique Parc de la Tête d'Or. Trois scènes simultanées, food trucks, espace détente et animations pour toute la famille. Une journée de musique sous le soleil lyonnais.",
                    DateEvenement         = DateTime.Now.AddDays(22),
                    Lieu                  = "Parc de la Tête d'Or, 69006 Lyon",
                    Categorie             = "Concert",
                    NombrePlacesTotal     = 2000,
                    NombrePlacesRestantes = 1543,
                    PrixBase              = 12.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1514320291840-2e0a9bf2a9ae?w=800",
                    OrganisateurId        = orga2.Id
                },
                new Evenement
                {
                    Titre                 = "Rock en Seine — Bordeaux",
                    Description           = "Le grand retour du festival rock sur les bords de la Garonne. Groupes locaux et têtes d'affiche nationales se succèdent sur deux scènes. Ambiance rock, bières artisanales et coucher de soleil sur la Garonne.",
                    DateEvenement         = DateTime.Now.AddDays(35),
                    Lieu                  = "Les Quais de Bordeaux, 33000 Bordeaux",
                    Categorie             = "Concert",
                    NombrePlacesTotal     = 1500,
                    NombrePlacesRestantes = 987,
                    PrixBase              = 25.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1501386761578-eac5c94b800a?w=800",
                    OrganisateurId        = orga3.Id
                },
            };

            // ============================================================
            // ÉVÉNEMENTS — SOIRÉES (3)
            // ============================================================

            var soirees = new List<Evenement>
            {
                new Evenement
                {
                    Titre                 = "Soirée Rooftop — Marseille",
                    Description           = "Soirée exclusive sur le rooftop du Grand Hôtel avec vue panoramique sur le Vieux-Port. DJ set, cocktails premium et dress code élégant obligatoire. Places très limitées pour une expérience intime et raffinée.",
                    DateEvenement         = DateTime.Now.AddDays(10),
                    Lieu                  = "Grand Hôtel Rooftop, 4 La Canebière, 13001 Marseille",
                    Categorie             = "Soirée",
                    NombrePlacesTotal     = 80,
                    NombrePlacesRestantes = 17,
                    PrixBase              = 35.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1566417713940-fe7c737a9ef2?w=800",
                    OrganisateurId        = orga1.Id
                },
                new Evenement
                {
                    Titre                 = "Bal Masqué Vénitien",
                    Description           = "Une soirée hors du temps dans un château du XVIIIe siècle. Costumes vénitiens exigés, orchestre baroque en live, souper gastronomique inclus et révélation des masques à minuit. Une expérience unique et inoubliable.",
                    DateEvenement         = DateTime.Now.AddDays(28),
                    Lieu                  = "Château de Vaux-le-Vicomte, 77950 Maincy",
                    Categorie             = "Soirée",
                    NombrePlacesTotal     = 150,
                    NombrePlacesRestantes = 89,
                    PrixBase              = 75.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1541956064528-ac3b6c7b5552?w=800",
                    OrganisateurId        = orga2.Id
                },
                new Evenement
                {
                    Titre                 = "Soirée Privée — Club Underground",
                    Description           = "Soirée électro underground dans un lieu secret révélé 24h avant. Line-up surprise avec artistes reconnus de la scène techno européenne. Accès sur liste uniquement.",
                    DateEvenement         = DateTime.Now.AddDays(8),
                    Lieu                  = "Lieu révélé 24h avant — Paris 20e",
                    Categorie             = "Soirée",
                    NombrePlacesTotal     = 200,
                    NombrePlacesRestantes = 45,
                    PrixBase              = 20.00m,
                    Statut                = "Validé",
                    EstPrive              = true,
                    MotDePassePrive       = "underground2026",
                    ImageUrl              = "https://images.unsplash.com/photo-1574391884720-bbc3740c59d1?w=800",
                    OrganisateurId        = orga3.Id
                },
            };

            // ============================================================
            // ÉVÉNEMENTS — FESTIVALS (3)
            // ============================================================

            var festivals = new List<Evenement>
            {
                new Evenement
                {
                    Titre                 = "Festival des Cultures du Monde",
                    Description           = "Trois jours de musiques, danses et gastronomies du monde entier. Plus de 40 pays représentés, scènes dédiées, village artisanal, ateliers et conférences. Un voyage immobile au cœur de Nantes.",
                    DateEvenement         = DateTime.Now.AddDays(45),
                    Lieu                  = "Île de Nantes, 44200 Nantes",
                    Categorie             = "Festival",
                    NombrePlacesTotal     = 5000,
                    NombrePlacesRestantes = 3210,
                    PrixBase              = 30.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?w=800",
                    OrganisateurId        = orga1.Id
                },
                new Evenement
                {
                    Titre                 = "Festival Électronique — Strasbourg",
                    Description           = "Le plus grand festival électronique d'Alsace investit les anciennes usines du port du Rhin. 5 scènes, 48h non-stop, camping sur place, food village et market artistique. Pass 1 jour ou 2 jours disponibles.",
                    DateEvenement         = DateTime.Now.AddDays(60),
                    Lieu                  = "Port du Rhin, 67100 Strasbourg",
                    Categorie             = "Festival",
                    NombrePlacesTotal     = 8000,
                    NombrePlacesRestantes = 5430,
                    PrixBase              = 45.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1492684223066-81342ee5ff30?w=800",
                    OrganisateurId        = orga2.Id
                },
                new Evenement
                {
                    Titre                 = "Festival Cinéma en Plein Air",
                    Description           = "Une semaine de projections gratuites sous les étoiles dans le parc du Château. Films cultes, avant-premières, rencontres avec des réalisateurs et animations culturelles. Apportez votre plaid et profitez du grand écran en pleine nature.",
                    DateEvenement         = DateTime.Now.AddDays(20),
                    Lieu                  = "Parc du Château de Versailles, 78000 Versailles",
                    Categorie             = "Festival",
                    NombrePlacesTotal     = 3000,
                    NombrePlacesRestantes = 2100,
                    PrixBase              = 0.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1478720568477-152d9b164e26?w=800",
                    OrganisateurId        = orga3.Id
                },
            };

            // ============================================================
            // ÉVÉNEMENTS — SPECTACLES (3)
            // ============================================================

            var spectacles = new List<Evenement>
            {
                new Evenement
                {
                    Titre                 = "Gala de Danse Contemporaine",
                    Description           = "La compagnie nationale présente son nouveau spectacle mêlant danse contemporaine, vidéo mapping et musique électro-acoustique. Une heure trente de pure émotion chorégraphique dans un théâtre à l'italienne classé.",
                    DateEvenement         = DateTime.Now.AddDays(18),
                    Lieu                  = "Théâtre National de Chaillot, 75016 Paris",
                    Categorie             = "Spectacle",
                    NombrePlacesTotal     = 400,
                    NombrePlacesRestantes = 203,
                    PrixBase              = 28.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1518834107812-67b0b7c58434?w=800",
                    OrganisateurId        = orga1.Id
                },
                new Evenement
                {
                    Titre                 = "One Man Show — Kev Adams",
                    Description           = "Le retour de l'humoriste préféré des Français avec un tout nouveau spectacle. Deux heures de rires garantis, anecdotes personnelles et sketches inédits. Attention, les places partent très vite !",
                    DateEvenement         = DateTime.Now.AddDays(12),
                    Lieu                  = "Zenith de Paris, 75019 Paris",
                    Categorie             = "Spectacle",
                    NombrePlacesTotal     = 3000,
                    NombrePlacesRestantes = 12,
                    PrixBase              = 42.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1527529482837-4698179dc6ce?w=800",
                    OrganisateurId        = orga2.Id
                },
                new Evenement
                {
                    Titre                 = "Cirque Moderne — Nouvelle Génération",
                    Description           = "Une troupe internationale de 30 artistes réinvente le cirque avec acrobaties aériennes, jonglerie de feu et clown poétique. Spectacle tout public dès 6 ans. Représentations quotidiennes à 17h et 20h30.",
                    DateEvenement         = DateTime.Now.AddDays(5),
                    Lieu                  = "Chapiteau de la Villette, 75019 Paris",
                    Categorie             = "Spectacle",
                    NombrePlacesTotal     = 600,
                    NombrePlacesRestantes = 234,
                    PrixBase              = 22.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800",
                    OrganisateurId        = orga3.Id
                },
            };

            // ============================================================
            // ÉVÉNEMENTS — FORMATIONS (3)
            // ============================================================

            var formations = new List<Evenement>
            {
                new Evenement
                {
                    Titre                 = "Masterclass Photographie — Argentique",
                    Description           = "Une journée complète dédiée à la photographie argentique avec un photographe professionnel reconnu. Théorie le matin, pratique l'après-midi en studio et en extérieur. Pellicule et développement inclus. Niveau débutant à intermédiaire.",
                    DateEvenement         = DateTime.Now.AddDays(14),
                    Lieu                  = "Studio Photo Pro, 15 rue des Arts, 69001 Lyon",
                    Categorie             = "Formation",
                    NombrePlacesTotal     = 12,
                    NombrePlacesRestantes = 4,
                    PrixBase              = 120.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1452587925148-ce544e77e70d?w=800",
                    OrganisateurId        = orga1.Id
                },
                new Evenement
                {
                    Titre                 = "Workshop Développement Web — React & Node",
                    Description           = "Formation intensive de 2 jours sur les technologies React et Node.js. Développement d'une application complète de A à Z. Prérequis : notions de JavaScript. Ordinateur portable obligatoire. Attestation de formation remise en fin de stage.",
                    DateEvenement         = DateTime.Now.AddDays(25),
                    Lieu                  = "Digital Campus, 24 rue de la Paix, 75002 Paris",
                    Categorie             = "Formation",
                    NombrePlacesTotal     = 20,
                    NombrePlacesRestantes = 8,
                    PrixBase              = 299.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1461749280684-dccba630e2f6?w=800",
                    OrganisateurId        = orga2.Id
                },
                new Evenement
                {
                    Titre                 = "Séminaire Leadership & Management",
                    Description           = "Une journée de séminaire dédiée au développement du leadership et aux nouvelles méthodes de management agile. Intervenants issus des plus grandes entreprises françaises. Networking, ateliers pratiques et remise d'un cahier de ressources exclusif.",
                    DateEvenement         = DateTime.Now.AddDays(30),
                    Lieu                  = "Palais des Congrès, Place de la Porte Maillot, 75017 Paris",
                    Categorie             = "Formation",
                    NombrePlacesTotal     = 150,
                    NombrePlacesRestantes = 67,
                    PrixBase              = 189.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1515187029135-18ee286d815b?w=800",
                    OrganisateurId        = orga3.Id
                },
            };

            // ============================================================
            // ÉVÉNEMENTS — CONVENTIONS (3)
            // ============================================================

            var conventions = new List<Evenement>
            {
                new Evenement
                {
                    Titre                 = "Japan Expo — Paris",
                    Description           = "Le plus grand événement dédié à la culture japonaise en Europe : manga, anime, cosplay, jeux vidéo, gastronomie et arts traditionnels. 3 jours de découverte avec plus de 700 exposants et des guests internationaux.",
                    DateEvenement         = DateTime.Now.AddDays(50),
                    Lieu                  = "Paris Le Bourget, 93350 Le Bourget",
                    Categorie             = "Convention",
                    NombrePlacesTotal     = 20000,
                    NombrePlacesRestantes = 12450,
                    PrixBase              = 15.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800",
                    OrganisateurId        = orga1.Id
                },
                new Evenement
                {
                    Titre                 = "Comic Con Toulouse",
                    Description           = "La convention pop culture du Sud-Ouest réunit fans de comics, séries, jeux de rôle et cosplay. Dédicaces avec des auteurs et artistes, concours de costumes, tournois de jeux de société et espace retrogaming.",
                    DateEvenement         = DateTime.Now.AddDays(40),
                    Lieu                  = "Parc des Expositions, 31100 Toulouse",
                    Categorie             = "Convention",
                    NombrePlacesTotal     = 5000,
                    NombrePlacesRestantes = 3120,
                    PrixBase              = 12.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1608889175157-b6b9e4e8e28e?w=800",
                    OrganisateurId        = orga2.Id
                },
                new Evenement
                {
                    Titre                 = "Salon du Livre & BD — Angoulême",
                    Description           = "Le rendez-vous incontournable des amateurs de littérature et de bande dessinée. Rencontres avec des auteurs, séances de dédicaces, expositions originales et conférences thématiques. Entrée libre pour les moins de 12 ans.",
                    DateEvenement         = DateTime.Now.AddDays(55),
                    Lieu                  = "Espace Franquin, 16000 Angoulême",
                    Categorie             = "Convention",
                    NombrePlacesTotal     = 3000,
                    NombrePlacesRestantes = 2345,
                    PrixBase              = 8.00m,
                    Statut                = "Validé",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1481627834876-b7833e8f5570?w=800",
                    OrganisateurId        = orga3.Id
                },
            };

            // ============================================================
            // ÉVÉNEMENT EN ATTENTE (pour tester le dashboard admin)
            // ============================================================

            var enAttente = new List<Evenement>
            {
                new Evenement
                {
                    Titre                 = "Soirée Années 80 — À valider",
                    Description           = "Soirée thématique années 80 avec groupe live reprenant les plus grands tubes de la décennie. Concours de tenues rétro avec lot à gagner. Bar à cocktails rétro et photobooth.",
                    DateEvenement         = DateTime.Now.AddDays(32),
                    Lieu                  = "La Bellevilloise, 19-21 rue Boyer, 75020 Paris",
                    Categorie             = "Soirée",
                    NombrePlacesTotal     = 300,
                    NombrePlacesRestantes = 300,
                    PrixBase              = 15.00m,
                    Statut                = "EnAttente",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1504680177321-2e6a879aac86?w=800",
                    OrganisateurId        = orga1.Id
                },
                new Evenement
                {
                    Titre                 = "Concert Hip-Hop — À valider",
                    Description           = "Battle de rap et concert hip-hop avec des artistes locaux et un headliner surprise. Freestyle, beatbox et graffiti live. Entrée pas chère, ambiance garantie.",
                    DateEvenement         = DateTime.Now.AddDays(19),
                    Lieu                  = "Le Bataclan, 50 Bd Voltaire, 75011 Paris",
                    Categorie             = "Concert",
                    NombrePlacesTotal     = 1200,
                    NombrePlacesRestantes = 1200,
                    PrixBase              = 22.00m,
                    Statut                = "EnAttente",
                    EstPrive              = false,
                    ImageUrl              = "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=800",
                    OrganisateurId        = orga2.Id
                },
            };

            // Sauvegarde tous les événements
            context.Evenements.AddRange(concerts);
            context.Evenements.AddRange(soirees);
            context.Evenements.AddRange(festivals);
            context.Evenements.AddRange(spectacles);
            context.Evenements.AddRange(formations);
            context.Evenements.AddRange(conventions);
            context.Evenements.AddRange(enAttente);
            await context.SaveChangesAsync();

            // ============================================================
            // BILLETS D'EXEMPLE (pour tester l'espace personnel)
            // ============================================================

            var billets = new List<Billet>
            {
                new Billet
                {
                    CodeQR      = Guid.NewGuid().ToString(),
                    TypeBillet  = "Standard",
                    PrixPaye    = concerts[0].PrixBase,
                    StatutScan  = "NonScanné",
                    AcheteurId  = acheteur1.Id,
                    EvenementId = concerts[0].Id,
                    DateAchat   = DateTime.Now.AddDays(-2)
                },
                new Billet
                {
                    CodeQR      = Guid.NewGuid().ToString(),
                    TypeBillet  = "VIP",
                    PrixPaye    = spectacles[0].PrixBase * 2,
                    StatutScan  = "NonScanné",
                    AcheteurId  = acheteur1.Id,
                    EvenementId = spectacles[0].Id,
                    DateAchat   = DateTime.Now.AddDays(-5)
                },
                new Billet
                {
                    CodeQR      = Guid.NewGuid().ToString(),
                    TypeBillet  = "Standard",
                    PrixPaye    = festivals[0].PrixBase,
                    StatutScan  = "Validé",
                    AcheteurId  = acheteur2.Id,
                    EvenementId = festivals[0].Id,
                    DateAchat   = DateTime.Now.AddDays(-10)
                },
                new Billet
                {
                    CodeQR      = Guid.NewGuid().ToString(),
                    TypeBillet  = "Réduit",
                    PrixPaye    = formations[0].PrixBase * 0.5m,
                    StatutScan  = "NonScanné",
                    AcheteurId  = acheteur2.Id,
                    EvenementId = formations[0].Id,
                    DateAchat   = DateTime.Now.AddDays(-1)
                },
            };

            context.Billets.AddRange(billets);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Seed terminé !");
            Console.WriteLine($"   → {await context.Utilisateurs.CountAsync()} utilisateurs");
            Console.WriteLine($"   → {await context.Evenements.CountAsync()} événements");
            Console.WriteLine($"   → {await context.Billets.CountAsync()} billets");
        }
    }
}