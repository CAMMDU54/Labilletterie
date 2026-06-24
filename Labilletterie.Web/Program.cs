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

// Sessions (pour événements privés)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Enregistrement des services
builder.Services.AddScoped<Labilletterie.Services.QrCodeService>();
builder.Services.AddScoped<Labilletterie.Services.PdfBilletService>();

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ---- SEED DES DONNÉES (à retirer en production) ----
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    // Lance le seed uniquement si on passe l'argument --seed
    if (args.Contains("--seed"))
    {
        Console.WriteLine("🌱 Lancement du seed...");
        await Labilletterie.Data.SeedData.InitialiserAsync(context);
    }
}

app.Run();

app.Run();