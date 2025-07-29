using System.ComponentModel.DataAnnotations;

namespace VenusBeautyStore.PL.Models.ViewModels;

public class UsuarioInternoViewModel
{

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un rol")]
    public string Rol { get; set; }
}
