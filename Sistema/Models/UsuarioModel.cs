using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema.Models
{
    public class UsuarioModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Usuario es obligatorio")]
        public string? Usuario { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es obligatorio")]
        [DataType(DataType.Password)]
        public string? Contrasena { get; set; }

        [Required(ErrorMessage = "El campo Rol es obligatorio")]
        public string? Rol { get; set; } // "RecursosHumanos" o "DireccionGeneral"
    }
}
