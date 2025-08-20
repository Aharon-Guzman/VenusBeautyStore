using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
                          UserManager<ApplicationUser> userManager,
                          ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string ReturnUrl { get; set; } = "/";

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required, DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Recordarme")]
            public bool RememberMe { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            if (!ModelState.IsValid) return Page();

            // Buscar usuario por email
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user is null)
            {
                // Mensaje genérico para no revelar si existe o no
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return Page();
            }

            // Requiere confirmación de correo
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Debes confirmar tu correo antes de iniciar sesión.");
                return Page();
            }

            // Iniciar sesión (cuenta intentos fallidos)
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario inició sesión.");

                //  Redirección por rol: staff al Portal
                if (await _userManager.IsInRoleAsync(user, "Admin") ||
                    await _userManager.IsInRoleAsync(user, "Recepcionista") ||
                    await _userManager.IsInRoleAsync(user, "Estilista"))
                {
                    // En PageModel no hay RedirectToAction; usamos Url.Action + Redirect
                    var portalUrl = Url.Action("Index", "Portal") ?? Url.Content("~/Portal");
                    return Redirect(portalUrl);
                }

                // Cliente u otros roles: respeta returnUrl si es local
                if (Url.IsLocalUrl(ReturnUrl))
                    return LocalRedirect(ReturnUrl);

                return Redirect(Url.Content("~/"));
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Usuario bloqueado por demasiados intentos.");
                return RedirectToPage("./ForgotPassword", new { email = Input.Email });
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl, Input.RememberMe });
            }

            ModelState.AddModelError(string.Empty, "Intento de inicio de sesión inválido.");
            return Page();
        }
    }
}
