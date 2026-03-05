# Changelog — Labilletterie

## [1.0.0] — 2026-03-05

### ✅ Fonctionnalités ajoutées
- Structure du projet ASP.NET Core MVC + SQLite
- Authentification (inscription, connexion, déconnexion) avec cookies
- Rôles utilisateurs : Acheteur, Organisateur, Admin, SuperAdmin
- Page d'accueil premium (hero, catégories, cards événements)
- Gestion des événements (création, liste, détail, filtres, recherche)
- Événements privés avec mot de passe
- Dashboard administrateur (validation, refus, stats globales)
- Gestion des utilisateurs (activation, changement de rôle)
- Achat de billets (Standard, VIP, Réduit)
- Génération automatique de QR code unique par billet
- Téléchargement du billet en PDF
- Espace personnel acheteur (historique, points fidélité)
- Scan QR de contrôle d'accès (caméra + saisie manuelle)
- Journal des scans en temps réel
- Programme fidélité (1€ = 1 point)
- Design CSS premium sombre (noir + bordeaux)
- 100% responsive mobile-first

### 🧪 Tests
- 28 tests unitaires (xUnit)
- Couverture : Utilisateur, Événement, Billet, QR Code, Scan

### 🔧 Stack technique
- C# / ASP.NET Core MVC (.NET 10)
- SQLite + Entity Framework Core
- HTML5 sémantique + CSS3 (Flex, Grid, Variables)
- BCrypt (mots de passe)
- QRCoder (génération QR)
- QuestPDF (génération PDF)
- jsQR (lecture QR caméra)
- Git + Git Flow + GitHub