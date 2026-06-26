# 🎟️ Labilletterie

Plateforme de billetterie en ligne premium, moderne et responsive.

## 🚀 Démarrage rapide

### Prérequis
- .NET 10 SDK
- Git

### Installation
```bash
# Cloner le projet
git clone git@github.com:Velya54/Labilletterie.git
cd labilletterie/Labilletterie

# Restaurer les packages
dotnet restore

# Créer la base de données
dotnet ef database update

# Lancer le projet
dotnet run
```

Ouvre `hhttp://localhost:5139` dans ton navigateur.

## 👥 Rôles utilisateurs

| Rôle | Accès |
|------|-------|
| Acheteur | Acheter des billets, espace personnel |
| Organisateur | Créer des événements, scanner les billets |
| Admin | Valider les événements, gérer les utilisateurs |
| SuperAdmin | Accès complet |

## 🛠️ Stack technique

- **Back** : C# / ASP.NET Core MVC / Entity Framework Core
- **Base de données** : SQLite
- **Front** : HTML5 / CSS3 / JavaScript vanilla
- **Authentification** : Cookies ASP.NET
- **QR Code** : QRCoder + jsQR
- **PDF** : QuestPDF
- **Tests** : xUnit

## 📁 Structure du projet
```
Labilletterie/
├── Controllers/      ← Logique métier
├── Models/           ← Modèles de données
├── ViewModels/       ← Modèles de formulaires
├── Views/            ← Pages HTML (Razor)
├── Services/         ← Services (QR, PDF)
├── Data/             ← DbContext SQLite
└── wwwroot/          ← CSS, JS, images

## 📋 Roadmap

- [x] Phase 1 : Authentification & rôles
- [x] Phase 2 : Gestion des événements
- [x] Phase 3 : Achat de billets & QR codes
- [x] Phase 4 : Dashboard admin
- [x] Phase 5 : Scan QR contrôle d'accès
- [ ] Phase 6 : Paiement Stripe
- [ ] Phase 7 : Newsletter & emails automatiques
- [ ] Phase 8 : Application mobile