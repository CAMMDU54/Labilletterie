// Services/PdfBilletService.cs

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Labilletterie.Models;

namespace Labilletterie.Services
{
    public class PdfBilletService
    {
        private readonly QrCodeService _qrCodeService;

        public PdfBilletService(QrCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }

        // Génère un PDF de billet et retourne les bytes
        [Obsolete]
        public byte[] GenererPdfBillet(Billet billet)
        {
            // Licence QuestPDF (communauté = gratuit)
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Format A5 paysage
                    page.Size(PageSizes.A5.Landscape());
                    page.Margin(30);
                    page.Background(Color.FromHex("#000000"));
                    page.DefaultTextStyle(t => t
                        .FontFamily("Arial")
                        .FontColor(Colors.White));

                    page.Content().Column(col =>
                    {
                        // ---- EN-TÊTE ----
                        col.Item().Row(row =>
                        {
                            // Logo / Nom plateforme
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Labilletterie")
                                    .FontSize(22)
                                    .Bold()
                                    .FontColor(Color.FromHex("#6b2835"));

                                c.Item().Text("Votre billet d'entrée")
                                    .FontSize(10)
                                    .FontColor(Color.FromHex("#aaaaaa"));
                            });

                            // Type de billet
                            row.ConstantItem(100).AlignRight().Text(billet.TypeBillet)
                                .FontSize(12)
                                .Bold()
                                .FontColor(Color.FromHex("#6b2835"));
                        });

                        col.Item().PaddingVertical(15).LineHorizontal(1)
                            .LineColor(Color.FromHex("#2a2a2a"));

                        // ---- INFOS ÉVÉNEMENT ----
                        col.Item().Row(row =>
                        {
                            // Infos gauche
                            row.RelativeItem().Column(c =>
                            {
                                // Titre événement
                                c.Item().Text(billet.Evenement?.Titre ?? "Événement")
                                    .FontSize(18)
                                    .Bold();

                                c.Item().PaddingTop(10).Text(
                                    $"📅 {billet.Evenement?.DateEvenement.ToString("dddd dd MMMM yyyy à HH:mm")}")
                                    .FontSize(10)
                                    .FontColor(Color.FromHex("#aaaaaa"));

                                c.Item().PaddingTop(5).Text(
                                    $"📍 {billet.Evenement?.Lieu ?? ""}")
                                    .FontSize(10)
                                    .FontColor(Color.FromHex("#aaaaaa"));

                                c.Item().PaddingTop(15).Text(
                                    $"👤 {billet.Acheteur?.Prenom} {billet.Acheteur?.Nom}")
                                    .FontSize(11)
                                    .Bold();

                                c.Item().PaddingTop(5).Text(
                                    $"Réf : {billet.CodeQR[..8].ToUpper()}")
                                    .FontSize(9)
                                    .FontColor(Color.FromHex("#aaaaaa"));
                            });

                            // QR Code droite
                            row.ConstantItem(110).Column(c =>
                            {
                                var qrBase64 = _qrCodeService
                                    .GenererQrCodeBase64(billet.CodeQR);
                                var qrBytes = Convert.FromBase64String(qrBase64);

                                c.Item().AlignCenter().Image(qrBytes)
                                    .FitWidth();

                                c.Item().AlignCenter().Text("Scanner à l'entrée")
                                    .FontSize(7)
                                    .FontColor(Color.FromHex("#aaaaaa"));
                            });
                        });

                        col.Item().PaddingVertical(15).LineHorizontal(1)
                            .LineColor(Color.FromHex("#2a2a2a"));

                        // ---- PIED ----
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text(
                                $"Prix payé : {billet.PrixPaye:0.00} €")
                                .FontSize(10)
                                .FontColor(Color.FromHex("#aaaaaa"));

                            row.RelativeItem().AlignRight().Text(
                                $"Acheté le {billet.DateAchat:dd/MM/yyyy}")
                                .FontSize(10)
                                .FontColor(Color.FromHex("#aaaaaa"));
                        });
                    });
                });
            }).GeneratePdf();
        }
    }
}