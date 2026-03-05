// Controllers/ScanController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Labilletterie.Data;
using Labilletterie.Models;

namespace Labilletterie.Controllers
{
    // Seuls les organisateurs et admins peuvent scanner
    [Authorize(Roles = "Organisateur,Admin,SuperAdmin")]
    public class ScanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // PAGE DE SCAN PRINCIPALE
        // GET /Scan
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Récupère les événements validés de l'organisateur
            var evenements = await _context.Evenements
                .Where(e => e.OrganisateurId == userId
                         && e.Statut == "Validé")
                .OrderBy(e => e.DateEvenement)
                .ToListAsync();

            // Les admins voient tous les événements validés
            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
            {
                evenements = await _context.Evenements
                    .Where(e => e.Statut == "Validé")
                    .OrderBy(e => e.DateEvenement)
                    .ToListAsync();
            }

            return View(evenements);
        }

        // ============================================================
        // PAGE DE SCAN D'UN ÉVÉNEMENT
        // GET /Scan/Evenement/{id}
        // ============================================================
        public async Task<IActionResult> Evenement(int id)
        {
            var evenement = await _context.Evenements
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evenement == null) return NotFound();

            // Journal des 20 derniers scans pour cet événement
            var scans = await _context.Scans
                .Include(s => s.Billet)
                    .ThenInclude(b => b!.Acheteur)
                .Include(s => s.ScannePar)
                .Where(s => s.Billet!.EvenementId == id)
                .OrderByDescending(s => s.DateScan)
                .Take(20)
                .ToListAsync();

            // Statistiques de l'événement
            int totalBillets = await _context.Billets
                .CountAsync(b => b.EvenementId == id);
            int billetsScannés = await _context.Billets
                .CountAsync(b => b.EvenementId == id
                             && b.StatutScan == "Validé");

            ViewBag.Evenement = evenement;
            ViewBag.TotalBillets = totalBillets;
            ViewBag.BilletsScannés = billetsScannés;
            ViewBag.Scans = scans;

            return View();
        }

        // ============================================================
        // API DE VALIDATION QR (appelée par le JavaScript)
        // POST /Scan/Valider
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Valider(
            [FromBody] ValiderQrRequest request)
        {
            var scanneurId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Cherche le billet par son code QR
            var billet = await _context.Billets
                .Include(b => b.Acheteur)
                .Include(b => b.Evenement)
                .FirstOrDefaultAsync(b => b.CodeQR == request.CodeQR);

            // Cas 1 : QR code inconnu
            if (billet == null)
            {
                return Json(new
                {
                    succes = false,
                    resultat = "Refusé",
                    message = "❌ QR code invalide ou inconnu",
                    couleur = "rouge"
                });
            }

            // Cas 2 : Billet pas pour cet événement
            if (billet.EvenementId != request.EvenementId)
            {
                return Json(new
                {
                    succes = false,
                    resultat = "Refusé",
                    message = "❌ Ce billet n'est pas valide pour cet événement",
                    couleur = "rouge"
                });
            }

            // Cas 3 : Billet déjà scanné
            if (billet.StatutScan == "Validé")
            {
                // Enregistre quand même le scan
                var scanDouble = new Scan
                {
                    BilletId = billet.Id,
                    Resultat = "DéjàScanné",
                    ScanneParId = scanneurId
                };
                _context.Scans.Add(scanDouble);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    succes = false,
                    resultat = "DéjàScanné",
                    message = $"⚠️ Billet déjà utilisé par {billet.Acheteur?.Prenom} {billet.Acheteur?.Nom}",
                    couleur = "orange",
                    acheteur = $"{billet.Acheteur?.Prenom} {billet.Acheteur?.Nom}",
                    typeBillet = billet.TypeBillet
                });
            }

            // Cas 4 : Billet valide → on valide l'entrée
            billet.StatutScan = "Validé";

            var scan = new Scan
            {
                BilletId = billet.Id,
                Resultat = "Validé",
                ScanneParId = scanneurId
            };

            _context.Scans.Add(scan);
            await _context.SaveChangesAsync();

            return Json(new
            {
                succes = true,
                resultat = "Validé",
                message = $"✅ Entrée validée pour {billet.Acheteur?.Prenom} {billet.Acheteur?.Nom}",
                couleur = "vert",
                acheteur = $"{billet.Acheteur?.Prenom} {billet.Acheteur?.Nom}",
                typeBillet = billet.TypeBillet
            });
        }

        // ============================================================
        // JOURNAL DES SCANS (API temps réel)
        // GET /Scan/Journal/{evenementId}
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Journal(int evenementId)
        {
            var scans = await _context.Scans
                .Include(s => s.Billet)
                    .ThenInclude(b => b!.Acheteur)
                .Where(s => s.Billet!.EvenementId == evenementId)
                .OrderByDescending(s => s.DateScan)
                .Take(10)
                .Select(s => new
                {
                    acheteur = $"{s.Billet!.Acheteur!.Prenom} {s.Billet.Acheteur.Nom}",
                    typeBillet = s.Billet.TypeBillet,
                    resultat = s.Resultat,
                    heure = s.DateScan.ToString("HH:mm:ss")
                })
                .ToListAsync();

            return Json(scans);
        }
    }

    // Modèle pour la requête de validation
    public class ValiderQrRequest
    {
        public string CodeQR { get; set; } = string.Empty;
        public int EvenementId { get; set; }
    }
}