// Models/Scan.cs

namespace Labilletterie.Models
{
    public class Scan
    {
        // Identifiant unique
        public int Id { get; set; }

        // Lien vers le billet scanné
        public int BilletId { get; set; }
        public Billet? Billet { get; set; }

        // Résultat du scan
        // "Validé", "DéjàScanné", "Refusé"
        public string Resultat { get; set; } = string.Empty;

        // Date et heure du scan
        public DateTime DateScan { get; set; } = DateTime.Now;

        // ID de l'organisateur/contrôleur qui a scanné
        public int ScanneParId { get; set; }
        public Utilisateur? ScannePar { get; set; }
    }
}