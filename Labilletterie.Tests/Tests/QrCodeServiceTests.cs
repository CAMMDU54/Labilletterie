// Tests/QrCodeServiceTests.cs

using Xunit;
using Labilletterie.Services;

namespace Labilletterie.Tests.Tests
{
    public class QrCodeServiceTests
    {
        private readonly QrCodeService _service;

        // Constructeur : instancie le service avant chaque test
        public QrCodeServiceTests()
        {
            _service = new QrCodeService();
        }

        // ============================================================
        // TEST 1 : Le QR code généré n'est pas vide
        // ============================================================
        [Fact]
        public void GenererQrCode_AvecContenu_RetourneBase64NonVide()
        {
            // ARRANGE
            string contenu = "test-qr-code-12345";

            // ACT
            string base64 = _service.GenererQrCodeBase64(contenu);

            // ASSERT
            Assert.NotNull(base64);
            Assert.NotEmpty(base64);
        }

        // ============================================================
        // TEST 2 : Le résultat est du base64 valide
        // ============================================================
        [Fact]
        public void GenererQrCode_Resultat_EstBase64Valide()
        {
            // ARRANGE
            string contenu = Guid.NewGuid().ToString();

            // ACT
            string base64 = _service.GenererQrCodeBase64(contenu);

            // ASSERT : On peut décoder le base64 sans erreur
            byte[] bytes = Convert.FromBase64String(base64);
            Assert.True(bytes.Length > 0);
        }

        // ============================================================
        // TEST 3 : Deux contenus différents → deux QR codes différents
        // ============================================================
        [Fact]
        public void GenererQrCode_ContenusDistincts_RetourneQrsDifferents()
        {
            // ARRANGE
            string contenu1 = "billet-uuid-001";
            string contenu2 = "billet-uuid-002";

            // ACT
            string qr1 = _service.GenererQrCodeBase64(contenu1);
            string qr2 = _service.GenererQrCodeBase64(contenu2);

            // ASSERT
            Assert.NotEqual(qr1, qr2);
        }

        // ============================================================
        // TEST 4 : Même contenu → même QR code (déterministe)
        // ============================================================
        [Fact]
        public void GenererQrCode_MemeContenu_RetourneMemeQr()
        {
            // ARRANGE
            string contenu = "billet-fixe-999";

            // ACT
            string qr1 = _service.GenererQrCodeBase64(contenu);
            string qr2 = _service.GenererQrCodeBase64(contenu);

            // ASSERT
            Assert.Equal(qr1, qr2);
        }
    }
}