// ViewModels/AchatViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace Labilletterie.ViewModels
{
    public class AchatViewModel
    {
        // ID de l'événement
        public int EvenementId { get; set; }

        // Type de billet choisi
        [Required(ErrorMessage = "Veuillez choisir un type de billet")]
        public string TypeBillet { get; set; } = "Standard";

        // Quantité de billets
        [Required]
        [Range(1, 10, ErrorMessage = "Entre 1 et 10 billets par commande")]
        [Display(Name = "Nombre de billets")]
        public int Quantite { get; set; } = 1;
    }
}