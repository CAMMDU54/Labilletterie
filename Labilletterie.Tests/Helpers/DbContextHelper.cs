// Helpers/DbContextHelper.cs

using Microsoft.EntityFrameworkCore;
using Labilletterie.Data;

namespace Labilletterie.Tests.Helpers
{
    public static class DbContextHelper
    {
        // Crée une base de données en mémoire fraîche pour chaque test
        // Chaque test a son propre nom unique pour être isolé
        public static ApplicationDbContext CreerContexteTest(string nomTest)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: nomTest)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}