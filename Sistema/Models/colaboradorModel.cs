using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sistema.Models
{
    public class ColaboradorModel
    {
      
        [Key]public int? num_empleado { get; set; }
       

        [DisplayName("Nombre")]
        [Required(ErrorMessage ="Escriba el nombre correctamente")]
        public string? nombre { get; set; }
       

        [DisplayName("Apellido")]
        [Required (ErrorMessage = "Escriba el apellido correctamente")]
        public string? apellido { get; set; }
        public List<SelectListItem>? apellidos { get; set; }

        public string? area { get; set; }

        [Required]
        public List<asistenciaModel> Asistencias { get; set; } = new List<asistenciaModel>();

        public static implicit operator ColaboradorModel(string v)
        {
            throw new NotImplementedException();
        }
    }
}
