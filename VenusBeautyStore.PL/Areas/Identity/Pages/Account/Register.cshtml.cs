using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using VenusBeauty.DAL.Context;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly VenusBeautyContext _context;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            VenusBeautyContext context,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string ReturnUrl { get; set; } = "/";

        public class InputModel
        {
            [Required, Display(Name = "Nombre")]
            public string Nombre { get; set; } = string.Empty;

            [Required, Display(Name = "Primer apellido")]
            public string Apellido1 { get; set; } = string.Empty;

            [Required, Display(Name = "Segundo apellido")]
            public string Apellido2 { get; set; } = string.Empty;

            [Required, Phone, Display(Name = "Teléfono")]
            public string Telefono { get; set; } = string.Empty;

            [Required, EmailAddress, Display(Name = "Correo electrónico")]
            public string Email { get; set; } = string.Empty;

            [Required, DataType(DataType.Password), Display(Name = "Contraseña")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password), Display(Name = "Confirmar contraseña"),
             Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? "/";
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
                return Page();

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                Nombres = Input.Nombre,
                Apellidos = $"{Input.Apellido1} {Input.Apellido2}",
                DisplayName = $"{Input.Nombre} {Input.Apellido1}",
                PhoneNumber = Input.Telefono,
                FotoUrl = ""
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                return Page();
            }

            _logger.LogInformation("Usuario Identity creado con ID: {Id}", user.Id);

            // Rol Cliente (por si acaso)
            if (!await _roleManager.RoleExistsAsync("Cliente"))
                await _roleManager.CreateAsync(new IdentityRole("Cliente"));
            await _userManager.AddToRoleAsync(user, "Cliente");

            // Crear registro en tabla Clientes
            try
            {
                var cliente = new Cliente
                {
                    Nombre = Input.Nombre,
                    Apellido1 = Input.Apellido1,
                    Apellido2 = Input.Apellido2,
                    Correo = Input.Email,
                    Telefono = Input.Telefono,
                    UserId = user.Id,
                    Activo = true
                };
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cliente guardado con Id {IdCliente}", cliente.IdCliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar Cliente");
            }

            // Generar token de confirmación y enviar email
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = encodedCode, returnUrl = ReturnUrl },
                protocol: Request.Scheme)!;

            var body = $@"
                    <div style=""font-family:Segoe UI,Arial,sans-serif;color:#111;font-size:15px;line-height:1.6"">
                        <p>¡Hola {HtmlEncoder.Default.Encode(Input.Nombre)}!</p>
                        <p>Gracias por registrarte en <strong>Venus Beauty</strong>.</p>
                        <p>Para activar tu cuenta, haz clic en el siguiente enlace:</p>
                        <p><a href=""{HtmlEncoder.Default.Encode(callbackUrl)}"">Confirmar mi correo</a></p>
                        <p>Si no fuiste tú, ignora este mensaje.</p>

                        <div style=""margin-top:20px;border-top:1px solid #e5e7eb;padding-top:16px"">
                            <a href=""https://venusbeautystore.com"" target=""_blank"" rel=""noopener"">
                                <img src=""cid:vbs-signature"" alt=""Soporte Venus Beauty Store""
                                     style=""max-width:520px;width:100%;height:auto;border-radius:12px;display:block;"" />
                            </a>
                        </div>
                    </div>";

            await _emailSender.SendEmailAsync(Input.Email, "Confirma tu cuenta - Venus Beauty", body);

            // NO iniciar sesión: se requiere confirmación
            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = ReturnUrl });
        }
    }
}
