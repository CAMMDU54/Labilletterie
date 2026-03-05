// Controllers/CompteController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Labilletterie.Data;
using Labilletterie.Models;
using Labilletterie.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Labilletterie.Controllers
{
    public class CompteController : Controller
    {
        // Injection du contexte de base de données
        private readonly ApplicationDbContext _context;

        public CompteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // INSCRIPTION
        // ============================================================

        // GET : affiche le formulaire d'inscription
        [HttpGet]
        public IActionResult Inscription()
        {
            return View();
        }

        // POST : traite le formulaire d'inscription
        [HttpPost]
        [ValidateAntiForgeryToken] // Protection contre les attaques CSRF
        public async Task<IActionResult> Inscription(InscriptionViewModel model)
        {
            // Vérifie que tous les champs sont valides
            if (!ModelState.IsValid)
                return View(model);

            // Vérifie si l'email existe déjà
            bool emailExiste = await _context.Utilisateurs
                .AnyAsync(u => u.Email == model.Email);

            if (emailExiste)
            {
                ModelState.AddModelError("Email", "Cet email est déjà utilisé");
                return View(model);
            }

            // Crée le nouvel utilisateur
            var utilisateur = new Utilisateur
            {
                Prenom = model.Prenom,
                Nom = model.Nom,
                Email = model.Email,
                // Hash du mot de passe avec BCrypt
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(model.MotDePasse),
                Role = model.Role
            };

            // Sauvegarde en base de données
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            // Connecte automatiquement après inscription
            await ConnecterUtilisateur(utilisateur);

            return RedirectToAction("Index", "Home");
        }

        // ============================================================
        // CONNEXION
        // ============================================================

        // GET : affiche le formulaire de connexion
        [HttpGet]
        public IActionResult Connexion()
        {
            return View();
        }

        // POST : traite le formulaire de connexion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Connexion(ConnexionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Recherche l'utilisateur par email
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            // Vérifie l'email ET le mot de passe
            if (utilisateur == null ||
                !BCrypt.Net.BCrypt.Verify(model.MotDePasse, utilisateur.MotDePasseHash))
            {
                ModelState.AddModelError("", "Email ou mot de passe incorrect");
                return View(model);
            }

            // Vérifie que le compte est actif
            if (!utilisateur.EstActif)
            {
                ModelState.AddModelError("", "Votre compte a été désactivé");
                return View(model);
            }

            // Connecte l'utilisateur
            await ConnecterUtilisateur(utilisateur, model.SeSouvenir);

            return RedirectToAction("Index", "Home");
        }

        // ============================================================
        // DÉCONNEXION
        // ============================================================

        public async Task<IActionResult> Deconnexion()
        {
            // Supprime le cookie d'authentification
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // ============================================================
        // MÉTHODE PRIVÉE : Connecter un utilisateur (créer le cookie)
        // ============================================================

        private async Task ConnecterUtilisateur(Utilisateur utilisateur, bool seSouvenir = false)
        {
            // Les "claims" = informations stockées dans le cookie
            var claims = new List<Claim>
            {
                // Identifiant unique
                new Claim(ClaimTypes.NameIdentifier, utilisateur.Id.ToString()),
                // Nom complet
                new Claim(ClaimTypes.Name, $"{utilisateur.Prenom} {utilisateur.Nom}"),
                // Email
                new Claim(ClaimTypes.Email, utilisateur.Email),
                // Rôle (très important pour les autorisations)
                new Claim(ClaimTypes.Role, utilisateur.Role)
            };

            var identite = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var proprietes = new AuthenticationProperties
            {
                // Cookie persistant si "Se souvenir de moi"
                IsPersistent = seSouvenir,
                ExpiresUtc = seSouvenir
                    ? DateTimeOffset.UtcNow.AddDays(30)  // 30 jours
                    : DateTimeOffset.UtcNow.AddHours(8)   // 8 heures
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identite),
                proprietes);
        }
    }
}