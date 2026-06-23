// Controllers/HomeController.cs

using Microsoft.AspNetCore.Mvc;
using Labilletterie.Data;
using Microsoft.EntityFrameworkCore;

namespace Labilletterie.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Page d'accueil
        public async Task<IActionResult> Index()
        {
            // Récupère les 6 derniers événements validés
            var evenements = await _context.Evenements
                .Where(e => e.Statut == "Validé" && !e.EstPrive)
                .OrderByDescending(e => e.DateCreation)
                .Take(6)
                .Include(e => e.Organisateur) // Charge aussi l'organisateur
                .ToListAsync();

            return View(evenements);
        }
    }
}