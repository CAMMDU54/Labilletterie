// Program.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Labilletterie.Data;

var builder = WebApplication.CreateBuilder(args);

// Connexion SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=labilletterie.db"));

// --- AJOUT : Authentification par cookies ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Page de connexion si non connecté
        options.LoginPath = "/Compte/Connexion";
        // Page si accès refusé (mauvais rôle)
        options.AccessDeniedPath = "/Compte/AccesRefuse";
        // Nom du cookie
        options.Cookie.Name = "Labilletterie.Auth";
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// --- IMPORTANT : l'ordre est obligatoire ---
app.UseAuthentication(); // Qui es-tu ?
app.UseAuthorization();  // As-tu le droit ?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();