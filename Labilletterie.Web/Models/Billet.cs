// Models/Billet.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Labilletterie.Models
{
    public class Billet
    {
        // Identifiant unique
        public int Id { get; set; }

        // Code QR unique généré automatiquement
        public string CodeQR { get; set; } = Guid.NewGuid().ToString();

        // Type : "Standard", "VIP", "Réduit", "Prévente"
        [MaxLength(50)]
        public string TypeBillet { get; set; } = "Standard";

        // Prix payé
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrixPaye { get; set; }

        // Statut : "NonScanné", "Validé", "DéjàScanné", "Refusé"
        public string StatutScan { get; set; } = "NonScanné";

        // Date d'achat
        public DateTime DateAchat { get; set; } = DateTime.Now;

        // Lien vers l'acheteur
        public int AcheteurId { get; set; }
        public Utilisateur? Acheteur { get; set; }

        // Lien vers l'événement
        public int EvenementId { get; set; }
        public Evenement? Evenement { get; set; }
    }
}