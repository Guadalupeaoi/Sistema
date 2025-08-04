using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sistema.Models
{
    public class ColaboradorModel
    {
        [DisplayName("Numero de empleado")]
        [Required(ErrorMessage = "Escriba el numero correctamente")]
        public int num_empleado { get; set; }
        public List<SelectListItem>? Numempleado { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage ="Escriba el nombre correctamente")]
        public string? nombre { get; set; }
        public List<SelectListItem>? nombres { get; set; }

        [DisplayName("Apellido")]
        [Required (ErrorMessage = "Escriba el apellido correctamente")]
        public string? apellido { get; set; }
        public List<SelectListItem>? apeliidos { get; set; }

        public string? area { get; set; }
    }
}
