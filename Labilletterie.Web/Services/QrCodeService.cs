// Services/QrCodeService.cs

using QRCoder;

namespace Labilletterie.Services
{
    public class QrCodeService
    {
        // Génère un QR code en base64 (image PNG encodée)
        // Le base64 peut être affiché directement dans une balise <img>
        public string GenererQrCodeBase64(string contenu)
        {
            // Crée le générateur de QR code
            using var qrGenerator = new QRCodeGenerator();

            // Génère les données du QR code à partir du contenu
            using var qrData = qrGenerator.CreateQrCode(
                contenu,
                QRCodeGenerator.ECCLevel.Q // Niveau de correction d'erreur
            );

            // Convertit en image PNG
            using var qrCode = new PngByteQRCode(qrData);
            byte[] imageBytes = qrCode.GetGraphic(10); // 10 = taille des pixels

            // Convertit en base64 pour l'affichage HTML
            return Convert.ToBase64String(imageBytes);
        }
    }
}