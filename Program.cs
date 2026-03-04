// Program.cs

using Microsoft.EntityFrameworkCore;
using Labilletterie.Data;

var builder = WebApplication.CreateBuilder(args);

// --- AJOUT : connexion à SQLite ---
// On indique à ASP.NET d'utiliser SQLite avec notre ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=labilletterie.db"));
// "labilletterie.db" = nom du fichier SQLite qui sera créé à la racine du projet

// Ajout des contrôleurs avec vues (déjà présent par défaut)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuration du pipeline HTTP (déjà présent par défaut)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();