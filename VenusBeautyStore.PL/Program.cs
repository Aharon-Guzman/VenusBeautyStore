using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VenusBeauty.DAL.Context;
using VenusBeauty.DAL.Repositories;
using VenusBeauty.BLL.Services;
using VenusBeautyStore.PL.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using VenusBeauty.DAL.Entities;

var builder = WebApplication.CreateBuilder(args);

// ✅ Cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ✅ Inyectar el contexto de base de datos
builder.Services.AddDbContext<VenusBeautyContext>(options =>
    options.UseSqlServer(connectionString));

// ✅ Identity (sin requerir confirmación de cuenta)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<VenusBeautyContext>();

// ✅ Registrar Repositories y Services
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
builder.Services.AddScoped<IServicioService, ServicioService>();


// ✅ Razor Pages e MVC
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// ✅ Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// ✅ Configuración de cultura (para usar coma como decimal)
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

// ✅ Configuración del pipeline HTTP
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
app.UseAuthentication();
app.UseAuthorization();

// ✅ Rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
