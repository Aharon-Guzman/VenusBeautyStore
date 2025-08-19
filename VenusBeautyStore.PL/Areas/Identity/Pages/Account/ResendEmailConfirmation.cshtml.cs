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
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required, EmailAddress]
            [Display(Name = "Correo electrónico")]
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

            // Por seguridad, respondemos igual aunque no exista
            if (user == null)
            {
                TempData["Info"] = "Si el correo existe, enviaremos el enlace de confirmación.";
                return RedirectToPage("./ResendEmailConfirmation");
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                TempData["Info"] = "Este correo ya está confirmado.";
                return RedirectToPage("./ResendEmailConfirmation");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = encoded },
                protocol: Request.Scheme
            )!;

            var body = $@"
        <p>Para confirmar tu cuenta, haz clic aquí:</p>
        <p><a href=""{HtmlEncoder.Default.Encode(callbackUrl)}"">Confirmar mi correo</a></p>";

            await _emailSender.SendEmailAsync(Input.Email, "Confirmar cuenta - Venus Beauty", body);

            TempData["Success"] = "Si el correo existe, enviamos nuevamente el enlace de confirmación.";
            return RedirectToPage("./ResendEmailConfirmation");
        }

    }
}
