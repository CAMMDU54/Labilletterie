// Data/ApplicationDbContext.cs

using Microsoft.EntityFrameworkCore;
using Labilletterie.Models;

namespace Labilletterie.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructeur : reçoit la configuration (chaîne de connexion SQLite)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Chaque DbSet = une table dans la base de données
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Evenement> Evenements { get; set; }
        public DbSet<Billet> Billets { get; set; }
    }
}