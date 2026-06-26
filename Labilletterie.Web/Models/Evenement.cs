// Models/Evenement.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Labilletterie.Models
{
    public class Evenement
    {
        // Identifiant unique
        public int Id { get; set; }

        // URL de l'image de l'événement (optionnelle)
        [MaxLength(500)]
        [Display(Name = "URL de l'image")]
        public string? ImageUrl { get; set; }

        // Titre de l'événement
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [MaxLength(200)]
        public string Titre { get; set; } = string.Empty;

        // Description détaillée
        public string Description { get; set; } = string.Empty;

        // Date et heure de l'événement
        [Required]
        public DateTime DateEvenement { get; set; }

        // Lieu / adresse
        [MaxLength(300)]
        public string Lieu { get; set; } = string.Empty;

        // Catégorie : "Concert", "Soirée", "Formation", "Spectacle"...
        [MaxLength(100)]
        public string Categorie { get; set; } = string.Empty;

        // Nombre total de places disponibles
        public int NombrePlacesTotal { get; set; }

        // Nombre de places restantes
        public int NombrePlacesRestantes { get; set; }

        // Prix de base du billet
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrixBase { get; set; }

        // Statut : "EnAttente", "Validé", "Refusé", "Annulé"
        public string Statut { get; set; } = "EnAttente";

        // Événement privé ou public
        public bool EstPrive { get; set; } = false;

        // Mot de passe si événement privé
        public string? MotDePassePrive { get; set; }

        // Date de création de l'annonce
        public DateTime DateCreation { get; set; } = DateTime.Now;

        // Lien vers l'organisateur (clé étrangère)
        public int OrganisateurId { get; set; }

        // Navigation : l'objet Utilisateur associé
        public Utilisateur? Organisateur { get; set; }

        // Un événement a plusieurs billets
        public List<Billet> Billets { get; set; } = new List<Billet>();
    }
}