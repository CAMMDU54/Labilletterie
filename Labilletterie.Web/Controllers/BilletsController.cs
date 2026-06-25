// Controllers/BilletsController.cs

using Labilletterie.Data;
using Labilletterie.Models;
using Labilletterie.Services;
using Labilletterie.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

            if (evenement.NombrePlacesRestantes <= 0)
            {
                TempData["Erreur"] = "Désolé, cet événement est complet.";
                return RedirectToAction("Detail", "Evenements",
                    new { id = evenementId });
            }

            // Passe l'événement ET un ViewModel vide à la vue
            ViewBag.Evenement = evenement;
            return View(new AchatViewModel { EvenementId = evenementId });
        }

        // ============================================================
        // CONFIRMATION D'ACHAT
        // POST /Billets/Confirmer
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmer(AchatViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Evenement = await _context.Evenements
                    .FirstOrDefaultAsync(e => e.Id == model.EvenementId);
                return View("Acheter", model);
            }

            var evenement = await _context.Evenements
                .FirstOrDefaultAsync(e => e.Id == model.EvenementId);

            if (evenement == null) return NotFound();

            // Vérifie qu'il y a assez de places pour la quantité demandée
            if (evenement.NombrePlacesRestantes < model.Quantite)
            {
                TempData["Erreur"] = $"Seulement {evenement.NombrePlacesRestantes} " +
                                      "place(s) disponible(s).";
                return RedirectToAction("Detail", "Evenements",
                    new { id = model.EvenementId });
            }

            var acheteurId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Calcul du prix unitaire selon le type
            decimal prixUnitaire = model.TypeBillet switch
            {
                "VIP" => evenement.PrixBase * 2,
                "Réduit" => evenement.PrixBase * 0.5m,
                _ => evenement.PrixBase
            };

            // Crée un billet par place demandée
            var billets = new List<Billet>();
            for (int i = 0; i < model.Quantite; i++)
            {
                billets.Add(new Billet
                {
                    CodeQR = Guid.NewGuid().ToString(),
                    TypeBillet = model.TypeBillet,
                    PrixPaye = prixUnitaire,
                    StatutScan = "NonScanné",
                    AcheteurId = acheteurId,
                    EvenementId = model.EvenementId,
                    DateAchat = DateTime.Now
                });
            }

            // Décrémente les places et ajoute les points fidélité
            evenement.NombrePlacesRestantes -= model.Quantite;

            var acheteur = await _context.Utilisateurs.FindAsync(acheteurId);
            if (acheteur != null)
                acheteur.PointsFidelite += (int)(prixUnitaire * model.Quantite);

            _context.Billets.AddRange(billets);
            await _context.SaveChangesAsync();

            // Si un seul billet → page billet individuelle
            // Si plusieurs → page récapitulatif de commande
            if (model.Quantite == 1)
                return RedirectToAction("MonBillet", new { id = billets[0].Id });

            return RedirectToAction("Confirmation",
                new { ids = string.Join(",", billets.Select(b => b.Id)) });
        }

        // Page de confirmation pour les achats multiples
        public async Task<IActionResult> Confirmation(string ids)
        {
            var acheteurId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Parse les IDs passés en paramètre
            var listIds = ids.Split(',')
                .Select(int.Parse)
                .ToList();

            var billets = await _context.Billets
                .Include(b => b.Evenement)
                .Include(b => b.Acheteur)
                .Where(b => listIds.Contains(b.Id)
                         && b.AcheteurId == acheteurId)
                .ToListAsync();

            if (!billets.Any()) return NotFound();

            return View(billets);
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