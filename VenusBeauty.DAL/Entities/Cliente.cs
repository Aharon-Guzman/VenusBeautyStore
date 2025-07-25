using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace VenusBeauty.DAL.Entities
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }
        [Required]
        [MaxLength(30)]
        public string? Nombre { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Apellido1 { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Apellido2 { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string? Correo { get; set; }

        [Required]
        [Phone]
        [MaxLength(15)]
        public string? Telefono { get; set; }


        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        public bool Activo { get; set; } = true;
    }
}
