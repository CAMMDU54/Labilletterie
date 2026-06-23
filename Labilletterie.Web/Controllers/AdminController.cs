// Controllers/AdminController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Labilletterie.Data;
using Labilletterie.Models;

namespace Labilletterie.Controllers
{
    // Seuls Admin et SuperAdmin peuvent accéder à ce contrôleur
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // TABLEAU DE BORD PRINCIPAL
        // GET /Admin
        // ============================================================
        public async Task<IActionResult> Index()
        {
            // Statistiques globales
            ViewBag.TotalEvenements = await _context.Evenements.CountAsync();
            ViewBag.EvenementsEnAttente = await _context.Evenements
                .CountAsync(e => e.Statut == "EnAttente");
            ViewBag.TotalUtilisateurs = await _context.Utilisateurs.CountAsync();
            ViewBag.TotalBillets = await _context.Billets.CountAsync();

            // Chiffre d'affaires total
            ViewBag.ChiffreAffaires = await _context.Billets
                .SumAsync(b => (decimal?)b.PrixPaye) ?? 0;

            // 5 derniers événements en attente
            var evenementsEnAttente = await _context.Evenements
                .Where(e => e.Statut == "EnAttente")
                .Include(e => e.Organisateur)
                .OrderByDescending(e => e.DateCreation)
                .Take(5)
                .ToListAsync();

            ViewBag.EvenementsEnAttenteList = evenementsEnAttente;

            return View();
        }

        // ============================================================
        // LISTE DES ÉVÉNEMENTS À VALIDER
        // GET /Admin/Evenements
        // ============================================================
        public async Task<IActionResult> Evenements(string? statut)
        {
            var query = _context.Evenements
                .Include(e => e.Organisateur)
                .AsQueryable();

            // Filtre par statut si précisé
            if (!string.IsNullOrEmpty(statut))
                query = query.Where(e => e.Statut == statut);

            var evenements = await query
                .OrderByDescending(e => e.DateCreation)
                .ToListAsync();

            ViewBag.StatutFiltre = statut;
            return View(evenements);
        }

        // ============================================================
        // VALIDER UN ÉVÉNEMENT
        // POST /Admin/Valider/{id}
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Valider(int id)
        {
            var evenement = await _context.Evenements.FindAsync(id);
            if (evenement == null) return NotFound();

            evenement.Statut = "Validé";
            await _context.SaveChangesAsync();

            TempData["Succes"] = $"L'événement \"{evenement.Titre}\" a été validé.";
            return RedirectToAction("Evenements");
        }

        // ============================================================
        // REFUSER UN ÉVÉNEMENT
        // POST /Admin/Refuser/{id}
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refuser(int id)
        {
            var evenement = await _context.Evenements.FindAsync(id);
            if (evenement == null) return NotFound();

            evenement.Statut = "Refusé";
            await _context.SaveChangesAsync();

            TempData["Erreur"] = $"L'événement \"{evenement.Titre}\" a été refusé.";
            return RedirectToAction("Evenements");
        }

        // ============================================================
        // LISTE DES UTILISATEURS
        // GET /Admin/Utilisateurs
        // ============================================================
        public async Task<IActionResult> Utilisateurs(string? recherche)
        {
            var query = _context.Utilisateurs.AsQueryable();

            if (!string.IsNullOrEmpty(recherche))
            {
                query = query.Where(u =>
                    u.Nom.Contains(recherche) ||
                    u.Prenom.Contains(recherche) ||
                    u.Email.Contains(recherche));
            }

            var utilisateurs = await query
                .OrderByDescending(u => u.DateCreation)
                .ToListAsync();

            ViewBag.Recherche = recherche;
            return View(utilisateurs);
        }

        // ============================================================
        // ACTIVER / DÉSACTIVER UN UTILISATEUR
        // POST /Admin/ToggleUtilisateur/{id}
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUtilisateur(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null) return NotFound();

            // Bascule entre actif et inactif
            utilisateur.EstActif = !utilisateur.EstActif;
            await _context.SaveChangesAsync();

            TempData["Succes"] = utilisateur.EstActif
                ? $"{utilisateur.Prenom} {utilisateur.Nom} a été réactivé."
                : $"{utilisateur.Prenom} {utilisateur.Nom} a été désactivé.";

            return RedirectToAction("Utilisateurs");
        }

        // ============================================================
        // CHANGER LE RÔLE D'UN UTILISATEUR
        // POST /Admin/ChangerRole/{id}
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangerRole(int id, string role)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null) return NotFound();

            // Seul SuperAdmin peut créer d'autres SuperAdmins
            if (role == "SuperAdmin" && !User.IsInRole("SuperAdmin"))
            {
                TempData["Erreur"] = "Vous n'avez pas les droits pour ce rôle.";
                return RedirectToAction("Utilisateurs");
            }

            utilisateur.Role = role;
            await _context.SaveChangesAsync();

            TempData["Succes"] = $"Rôle de {utilisateur.Prenom} changé en {role}.";
            return RedirectToAction("Utilisateurs");
        }
    }
}