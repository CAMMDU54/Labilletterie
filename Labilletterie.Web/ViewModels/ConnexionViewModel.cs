// ViewModels/ConnexionViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace Labilletterie.ViewModels
{
    public class ConnexionViewModel
    {
        // Email obligatoire
        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        [Display(Name = "Adresse email")]
        public string Email { get; set; } = string.Empty;

        // Mot de passe obligatoire
        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string MotDePasse { get; set; } = string.Empty;

        // Se souvenir de moi (cookie longue durée)
        [Display(Name = "Se souvenir de moi")]
        public bool SeSouvenir { get; set; } = false;
    }
}