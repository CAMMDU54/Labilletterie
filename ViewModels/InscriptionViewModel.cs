// ViewModels/InscriptionViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace Labilletterie.ViewModels
{
    public class InscriptionViewModel
    {
        // Prénom obligatoire
        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [MaxLength(50)]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; } = string.Empty;

        // Nom obligatoire
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [MaxLength(50)]
        [Display(Name = "Nom")]
        public string Nom { get; set; } = string.Empty;

        // Email valide et obligatoire
        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        [Display(Name = "Adresse email")]
        public string Email { get; set; } = string.Empty;

        // Mot de passe minimum 8 caractères
        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [MinLength(8, ErrorMessage = "8 caractères minimum")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string MotDePasse { get; set; } = string.Empty;

        // Confirmation du mot de passe
        [Required(ErrorMessage = "Confirmez votre mot de passe")]
        [DataType(DataType.Password)]
        [Compare("MotDePasse", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [Display(Name = "Confirmer le mot de passe")]
        public string ConfirmationMotDePasse { get; set; } = string.Empty;

        // Rôle choisi à l'inscription
        [Required]
        [Display(Name = "Je suis")]
        public string Role { get; set; } = "Acheteur";
    }
}