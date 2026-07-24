using Blazored.LocalStorage;
using Microsoft.AspNetCore.HttpOverrides;
using MudBlazor.Services;
using WedPlan.Components;
using WedPlan.Services;

var builder = WebApplication.CreateBuilder(args);

// Hosting-Plattformen wie Render geben den Port über die Umgebungsvariable PORT vor.
// Kestrel wird darauf gebunden; lokal bleibt das Standardverhalten erhalten.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://*:{port}");
}

// Hinter dem Reverse-Proxy der Hosting-Plattform (TLS wird dort terminiert)
// müssen weitergeleitete Header berücksichtigt werden, damit Redirects/HTTPS korrekt sind.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MudBlazor UI-Framework registrieren.
builder.Services.AddMudServices();

// Browser-LocalStorage für die clientseitige Datenhaltung registrieren.
builder.Services.AddBlazoredLocalStorage();

// Supabase-Verbindung (Cloud) und Authentifizierung registrieren.
// Scoped, damit jeder Benutzer-Circuit seine eigene Session/Client-Instanz hat.
builder.Services.AddScoped<SupabaseSessionPersistence>();
builder.Services.AddScoped<SupabaseClientProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Hält die aktive Hochzeit (wedding_id) für den aktuellen Benutzer-Circuit.
builder.Services.AddScoped<WeddingContext>();

// Verwaltung der Hochzeits-Gruppen (anlegen, beitreten, Mitglieder).
builder.Services.AddScoped<IWeddingService, WeddingService>();

// Rollen & seitenbasierte Bearbeitungsrechte des aktuellen Benutzers.
builder.Services.AddScoped<PermissionService>();

// Datei-Uploads (Titelbilder) über Supabase Storage.
builder.Services.AddScoped<IStorageService, StorageService>();

// Live-Synchronisation der Projekt-Einstellungen (z.B. Titelbild) via Supabase Realtime.
builder.Services.AddScoped<WeddingRealtimeService>();

// Fachliche Services registrieren. Über die Interfaces sind sie leicht austauschbar
// (z.B. später gegen EF-Core-/SQLite-Implementierungen).
// Scoped, da LocalStorage pro Benutzer-Circuit (Interactive Server) gilt.
builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IApartmentService, ApartmentService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<ISeatingService, SeatingService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IFinanceService, FinanceService>();

var app = builder.Build();

// Weitergeleitete Header (X-Forwarded-Proto/-For) vom Hosting-Proxy auswerten.
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

// TLS wird auf der Hosting-Plattform (Proxy) terminiert; lokal weiterhin HTTPS-Redirect.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
