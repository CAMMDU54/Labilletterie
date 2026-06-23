// Models/Utilisateur.cs

using System.ComponentModel.DataAnnotations; // Permet d'utiliser les validations

namespace Labilletterie.Models
{
    public class Utilisateur
    {
        // Clé primaire : identifiant unique de chaque utilisateur
        public int Id { get; set; }

        // Prénom obligatoire, 50 caractères max
        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [MaxLength(50)]
        public string Prenom { get; set; } = string.Empty;

        // Nom obligatoire, 50 caractères max
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [MaxLength(50)]
        public string Nom { get; set; } = string.Empty;

        // Email unique et obligatoire
        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Mot de passe hashé (jamais en clair !)
        [Required]
        public string MotDePasseHash { get; set; } = string.Empty;

        // Rôle : "Acheteur", "Organisateur", "Admin", "SuperAdmin"
        [Required]
        public string Role { get; set; } = "Acheteur";

        // Points fidélité cumulés
        public int PointsFidelite { get; set; } = 0;

        // Date de création du compte (automatique)
        public DateTime DateCreation { get; set; } = DateTime.Now;

        // Compte actif ou désactivé par un admin
        public bool EstActif { get; set; } = true;
    }
}