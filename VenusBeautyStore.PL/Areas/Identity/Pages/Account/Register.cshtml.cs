using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using VenusBeauty.DAL.Context;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly VenusBeautyContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            VenusBeautyContext context,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string ReturnUrl { get; set; } = "/";

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Correo electrónico")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? "/";
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("? Usuario Identity creado con ID: {Id}", user.Id);

                    try
                    {
                        var cliente = new Cliente
                        {
                            Nombre = "NombrePorDefecto",
                            Apellido1 = "Apellido1",
                            Apellido2 = "Apellido2",
                            Correo = Input.Email,
                            Telefono = "0000-0000",
                            UserId = user.Id,
                            Activo = true
                        };

                        _context.Clientes.Add(cliente);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("? Cliente guardado con Id {IdCliente}", cliente.IdCliente);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "? Error al insertar Cliente");
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
