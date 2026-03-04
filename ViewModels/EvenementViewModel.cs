// ViewModels/EvenementViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace Labilletterie.ViewModels
{
    public class EvenementViewModel
    {
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [MaxLength(200)]
        [Display(Name = "Titre de l'événement")]
        public string Titre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La description est obligatoire")]
        [Display(Name = "Description")]
        // Indique au champ HTML le format exact attendu
        [DataType(DataType.DateTime)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date est obligatoire")]
        [Display(Name = "Date et heure")]
        public DateTime DateEvenement { get; set; } = DateTime.Now.AddDays(7);

        [Required(ErrorMessage = "Le lieu est obligatoire")]
        [MaxLength(300)]
        [Display(Name = "Lieu / Adresse")]
        public string Lieu { get; set; } = string.Empty;

        [Required(ErrorMessage = "La catégorie est obligatoire")]
        [Display(Name = "Catégorie")]
        public string Categorie { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nombre de places est obligatoire")]
        [Range(1, 100000, ErrorMessage = "Entre 1 et 100 000 places")]
        [Display(Name = "Nombre de places")]
        public int NombrePlacesTotal { get; set; }

        [Required(ErrorMessage = "Le prix est obligatoire")]
        [Range(0, 10000, ErrorMessage = "Prix entre 0 et 10 000 €")]
        [Display(Name = "Prix du billet (€)")]
        public decimal PrixBase { get; set; }

        [Display(Name = "Événement privé ?")]
        public bool EstPrive { get; set; } = false;

        [Display(Name = "Mot de passe (si privé)")]
        public string? MotDePassePrive { get; set; }
    }
}