using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VenusBeauty.DAL.Context;
using VenusBeauty.DAL.Repositories;
using VenusBeauty.BLL.Services;
using VenusBeautyStore.PL.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using VenusBeauty.DAL.Entities;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ Cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ✅ DbContext
builder.Services.AddDbContext<VenusBeautyContext>(options =>
    options.UseSqlServer(connectionString));

// ✅ Identity (con Roles) + UI por defecto
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<VenusBeautyContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();   // 👈 importante para /Identity/Account/Login

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Email:Smtp"));
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
// ✅ Cookie: rutas correctas de Login/AccessDenied/Logout
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.LogoutPath = "/Identity/Account/Logout";
    options.SlidingExpiration = true;
});

// ✅ Repositories y Services
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
builder.Services.AddScoped<IServicioService, ServicioService>();
builder.Services.AddScoped<ITrabajadorRepository, TrabajadorRepository>();
builder.Services.AddScoped<ITrabajadorService, TrabajadorService>();
builder.Services.AddScoped<ICitaRepository, CitaRepository>();
builder.Services.AddScoped<ICitaService, CitaService>();

// ✅ Razor Pages (necesario para /Identity/Account/...) y MVC
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// ✅ Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// ✅ Cultura (es-CR)
var cultureInfo = new CultureInfo("es-CR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultureInfo),
    SupportedCultures = new List<CultureInfo> { cultureInfo },
    SupportedUICultures = new List<CultureInfo> { cultureInfo }
});

// ✅ Semilla de datos iniciales (roles/usuarios)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

// ✅ Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // 👈 antes de Authorization
app.UseAuthorization();

// ✅ Rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();       // 👈 necesario para /Identity/Account/*

app.Run();
