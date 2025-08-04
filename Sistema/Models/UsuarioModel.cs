using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema.Models
{
    public class UsuarioModel
    {
        [DisplayName("Usuario")]
        [Required(ErrorMessage = "El usuario no es correcto")]
        public string? NombreUsuario { get; set; }

        [DisplayName("Contrasela")]
        [Required(ErrorMessage = "Escriba la contraseña")]
        public string? Password { get; set; }
    }
}
