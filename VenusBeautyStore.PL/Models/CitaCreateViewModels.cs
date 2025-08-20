using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VenusBeautyStore.PL.Models
{
    /// <summary>
    /// Datos que necesita la vista Create de Citas
    /// </summary>
    public class CitaCreateViewModel
    {
        /* ---------- Campos que se envían al servidor ---------- */

        [Required(ErrorMessage = "Seleccione un cliente")]
        public int IdCliente { get; set; }             

        [Required(ErrorMessage = "Seleccione un estilista")]
        public string IdUsuario { get; set; } = null!;  

        [Required(ErrorMessage = "Indique la fecha y hora")]
        [DataType(DataType.DateTime)]
        public DateTime FechaHora { get; set; }

        // Lista de Ids de servicios marcados (checkbox)
        [Required(ErrorMessage = "Seleccione al menos un servicio")]
        public IEnumerable<int> ServiciosSeleccionados { get; set; } = new List<int>();

       
        public Dictionary<int, int> ProductosSeleccionados { get; set; } = new();

        /* ---------- Listas para poblar los controles ---------- */

        public IEnumerable<SelectListItem>? Clientes { get; set; }   
        public IEnumerable<SelectListItem>? Trabajadores { get; set; }
        public IEnumerable<SelectListItem>? Servicios { get; set; }
        public IEnumerable<SelectListItem>? Productos { get; set; }
    }
}
