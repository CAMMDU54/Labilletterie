// Controllers/BilletsController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Labilletterie.Data;
using Labilletterie.Models;
using Labilletterie.Services;

namespace Labilletterie.Controllers
{
    [Authorize] // Toutes les actions nécessitent d'être connecté
    public class BilletsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly QrCodeService _qrCodeService;
        private readonly PdfBilletService _pdfService;

        public BilletsController(
            ApplicationDbContext context,
            QrCodeService qrCodeService,
            PdfBilletService pdfService)
        {
            _context = context;
            _qrCodeService = qrCodeService;
            _pdfService = pdfService;
        }

        // ============================================================
        // PAGE D'ACHAT
        // GET /Billets/Acheter/{evenementId}
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Acheter(int evenementId)
        {
            var evenement = await _context.Evenements
                .Include(e => e.Organisateur)
                .FirstOrDefaultAsync(e => e.Id == evenementId);

            if (evenement == null) return NotFound();

            // Vérifie qu'il reste des places
            if (evenement.NombrePlacesRestantes <= 0)
            {
                TempData["Erreur"] = "Désolé, cet événement est complet.";
                return RedirectToAction("Detail", "Evenements",
                    new { id = evenementId });
            }

            return View(evenement);
        }

        // ============================================================
        // CONFIRMATION D'ACHAT
        // POST /Billets/Confirmer
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmer(int evenementId, string typeBillet)
        {
            var evenement = await _context.Evenements
                .FirstOrDefaultAsync(e => e.Id == evenementId);

            if (evenement == null) return NotFound();

            if (evenement.NombrePlacesRestantes <= 0)
            {
                TempData["Erreur"] = "Cet événement est complet.";
                return RedirectToAction("Index", "Evenements");
            }

            // Récupère l'ID de l'acheteur connecté
            var acheteurId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Calcule le prix selon le type de billet
            decimal prix = typeBillet switch
            {
                "VIP" => evenement.PrixBase * 2,
                "Réduit" => evenement.PrixBase * 0.5m,
                _ => evenement.PrixBase  // Standard
            };

            // Crée le billet
            var billet = new Billet
            {
                CodeQR = Guid.NewGuid().ToString(),
                TypeBillet = typeBillet,
                PrixPaye = prix,
                StatutScan = "NonScanné",
                AcheteurId = acheteurId,
                EvenementId = evenementId,
                DateAchat = DateTime.Now
            };

            // Décrémente le nombre de places restantes
            evenement.NombrePlacesRestantes--;

            // Ajoute des points fidélité (1€ = 1 point)
            var acheteur = await _context.Utilisateurs.FindAsync(acheteurId);
            if (acheteur != null)
            {
                acheteur.PointsFidelite += (int)prix;
            }

            _context.Billets.Add(billet);
            await _context.SaveChangesAsync();

            // Redirige vers la page du billet
            return RedirectToAction("MonBillet", new { id = billet.Id });
        }

        // ============================================================
        // PAGE DU BILLET (avec QR code)
        // GET /Billets/MonBillet/{id}
        // ============================================================
        public async Task<IActionResult> MonBillet(int id)
        {
            var acheteurId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var billet = await _context.Billets
                .Include(b => b.Evenement)
                .Include(b => b.Acheteur)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (billet == null) return NotFound();

            // Sécurité : seul l'acheteur peut voir son billet
            if (billet.AcheteurId != acheteurId &&
                !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
                return Forbid();

            // Génère le QR code en base64 pour l'affichage
            ViewBag.QrCodeBase64 = _qrCodeService
                .GenererQrCodeBase64(billet.CodeQR);

            return View(billet);
        }

        // ============================================================
        // TÉLÉCHARGER LE BILLET EN PDF
        // GET /Billets/Telecharger/{id}
        // ============================================================
        public async Task<IActionResult> Telecharger(int id)
        {
            var acheteurId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var billet = await _context.Billets
                .Include(b => b.Evenement)
                .Include(b => b.Acheteur)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (billet == null) return NotFound();

            if (billet.AcheteurId != acheteurId &&
                !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
                return Forbid();

            // Génère le PDF
            var pdfBytes = _pdfService.GenererPdfBillet(billet);

            // Retourne le fichier PDF au navigateur
            return File(
                pdfBytes,
                "application/pdf",
                $"billet-{billet.Id}-{billet.Evenement?.Titre}.pdf"
            );
        }

        // ============================================================
        // ESPACE PERSONNEL (historique des billets)
        // GET /Billets/MonEspace
        // ============================================================
        public async Task<IActionResult> MonEspace()
        {
            var acheteurId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var billets = await _context.Billets
                .Include(b => b.Evenement)
                .Where(b => b.AcheteurId == acheteurId)
                .OrderByDescending(b => b.DateAchat)
                .ToListAsync();

            // Infos de l'utilisateur
            var utilisateur = await _context.Utilisateurs
                .FindAsync(acheteurId);

            ViewBag.Utilisateur = utilisateur;

            return View(billets);
        }
    }
}