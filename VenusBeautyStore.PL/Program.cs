using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VenusBeauty.DAL.Context;
using VenusBeautyStore.PL.Data;

var builder = WebApplication.CreateBuilder(args);

// Cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Inyectar el contexto de base de datos principal (que contiene Identity + tus tablas)
builder.Services.AddDbContext<VenusBeautyContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<VenusBeautyContext>();

// Configurar Identity con confirmación de cuenta desactivada
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<VenusBeautyContext>();

// Activar soporte para páginas Razor personalizadas (como /Areas/Identity/Pages/Account/Register.cshtml)
builder.Services.AddRazorPages();

// Mostrar errores en desarrollo
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Agregar MVC
builder.Services.AddControllersWithViews();

// Configurar logging en consola
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services); // ✅ Llamamos al método Initialize
}

// Configurar pipeline HTTP
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
app.UseAuthentication(); // << ¡No te puede faltar!
app.UseAuthorization();

// Rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // << Necesario para Identity

app.Run();
