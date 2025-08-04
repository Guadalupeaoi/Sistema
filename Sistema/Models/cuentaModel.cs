using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sistema.Models
{
    public class cuentaModel
    {
        [DisplayName("Nombre de usuario")]
        [Required(ErrorMessage = "Inserte correctamente el usuario")]
        public string? nombreUsuario {  get; set; }

        [DisplayName("Contraseña")]
        [Required(ErrorMessage = "Escriba correctamente la contraseña")]
        public string? contraseña { get; set; }
    }
}
