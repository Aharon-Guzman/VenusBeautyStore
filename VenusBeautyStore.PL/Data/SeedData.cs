using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            //  Obtenemos RoleManager y UserManager desde los servicios
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            //  Definimos los roles base
            string[] roles = { "Admin", "Recepcionista", "Estilista" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"✅ Rol creado: {role}");
                }
            }

            //  Definimos el usuario Admin inicial
            string adminEmail = "admin@venusbeauty.com";
            string adminPassword = "Admin123$";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true, // Confirmado desde el inicio
                    Nombres = "Administrador",
                    Apellidos = "Principal",
                    DisplayName = "Administrador",
                };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                    Console.WriteLine("✅ Usuario Admin creado y asignado al rol Admin.");
                }
                else
                {
                    Console.WriteLine("❌ Error creando Admin: " + string.Join(", ", result.Errors));
                }
            }
            else
            {
                // Si ya existe, aseguramos que tenga el rol Admin
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
