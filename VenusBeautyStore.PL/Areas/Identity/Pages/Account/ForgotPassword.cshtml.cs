using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public void OnGet(string? email = null)
        {
            if (!string.IsNullOrWhiteSpace(email))
                Input.Email = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);

            // Por seguridad, siempre mostramos confirmación aunque no exista o no esté confirmado
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return RedirectToPage("./ForgotPasswordConfirmation");

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code = encodedCode, email = Input.Email },
                protocol: Request.Scheme)!;

            var body = $@"
                <p>Recibimos una solicitud para restablecer tu contraseña.</p>
                <p>Puedes hacerlo aquí: <a href=""{HtmlEncoder.Default.Encode(callbackUrl)}"">Restablecer contraseña</a></p>
                <p>Si no fuiste tú, ignora este mensaje.</p>";
                
            await _emailSender.SendEmailAsync(Input.Email, "Restablecer contraseña - Venus Beauty", body);

            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
