using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace VenusBeauty.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombres { get; set; } 
        public string Apellidos { get; set; }
        public string DisplayName { get; set; }
        public string FotoUrl { get; set; }
    }
}
