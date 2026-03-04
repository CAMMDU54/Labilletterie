// Controllers/EvenementsController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Labilletterie.Data;
using Labilletterie.Models;
using Labilletterie.ViewModels;

namespace Labilletterie.Controllers
{
    public class EvenementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EvenementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // LISTE DES ÉVÉNEMENTS
        // GET /Evenements
        // ============================================================
        public async Task<IActionResult> Index(string? recherche, string? categorie)
        {
            // On part de tous les événements validés et publics
            var query = _context.Evenements
                .Where(e => e.Statut == "Validé" && !e.EstPrive)
                .Include(e => e.Organisateur)
                .AsQueryable();

            // Filtre par recherche (titre ou lieu)
            if (!string.IsNullOrEmpty(recherche))
            {
                query = query.Where(e =>
                    e.Titre.Contains(recherche) ||
                    e.Lieu.Contains(recherche));
            }

            // Filtre par catégorie
            if (!string.IsNullOrEmpty(categorie))
            {
                query = query.Where(e => e.Categorie == categorie);
            }

            // Trie par date croissante
            var evenements = await query
                .OrderBy(e => e.DateEvenement)
                .ToListAsync();

            // Passe les filtres à la vue pour les réafficher
            ViewBag.Recherche = recherche;
            ViewBag.Categorie = categorie;

            return View(evenements);
        }

        // ============================================================
        // DÉTAIL D'UN ÉVÉNEMENT
        // GET /Evenements/{id}
        // ============================================================
        public async Task<IActionResult> Detail(int id)
        {
            var evenement = await _context.Evenements
                .Include(e => e.Organisateur)
                .FirstOrDefaultAsync(e => e.Id == id);

            // Si l'événement n'existe pas → page 404
            if (evenement == null)
                return NotFound();

            // Si événement privé → vérifie le mot de passe
            if (evenement.EstPrive)
            {
                string? mdpSession = HttpContext.Session.GetString($"prive_{id}");
                if (mdpSession != evenement.MotDePassePrive)
                    return RedirectToAction("AccesPrive", new { id });
            }

            return View(evenement);
        }

        // ============================================================
        // ACCÈS ÉVÉNEMENT PRIVÉ
        // GET /Evenements/AccesPrive/{id}
        // ============================================================
        [HttpGet]
        public IActionResult AccesPrive(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AccesPrive(int id, string motDePasse)
        {
            var evenement = await _context.Evenements
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evenement == null) return NotFound();

            if (evenement.MotDePassePrive == motDePasse)
            {
                // Stocke en session que l'accès est autorisé
                HttpContext.Session.SetString($"prive_{id}", motDePasse);
                return RedirectToAction("Detail", new { id });
            }

            ViewBag.Id = id;
            ViewBag.Erreur = "Mot de passe incorrect";
            return View();
        }

        // ============================================================
        // CRÉER UN ÉVÉNEMENT (organisateurs uniquement)
        // GET /Evenements/Creer
        // ============================================================
        [Authorize(Roles = "Organisateur,Admin,SuperAdmin")]
        [HttpGet]
        public IActionResult Creer()
        {
            return View(new EvenementViewModel());
        }

        [Authorize(Roles = "Organisateur,Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Creer(EvenementViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Récupère l'ID de l'utilisateur connecté
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var evenement = new Evenement
            {
                Titre = model.Titre,
                Description = model.Description,
                DateEvenement = model.DateEvenement,
                Lieu = model.Lieu,
                Categorie = model.Categorie,
                NombrePlacesTotal = model.NombrePlacesTotal,
                NombrePlacesRestantes = model.NombrePlacesTotal,
                PrixBase = model.PrixBase,
                EstPrive = model.EstPrive,
                MotDePassePrive = model.MotDePassePrive,
                OrganisateurId = userId,
                // En attente de validation par un admin
                Statut = "EnAttente"
            };

            _context.Evenements.Add(evenement);
            await _context.SaveChangesAsync();

            TempData["Succes"] = "Votre événement a été soumis et est en attente de validation.";
            return RedirectToAction("MesEvenements");
        }

        // ============================================================
        // MES ÉVÉNEMENTS (organisateur connecté)
        // GET /Evenements/MesEvenements
        // ============================================================
        [Authorize(Roles = "Organisateur,Admin,SuperAdmin")]
        public async Task<IActionResult> MesEvenements()
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var evenements = await _context.Evenements
                .Where(e => e.OrganisateurId == userId)
                .OrderByDescending(e => e.DateCreation)
                .ToListAsync();

            return View(evenements);
        }
    }
}