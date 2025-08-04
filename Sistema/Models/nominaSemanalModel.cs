using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema.Models
{
    public class nominaSemanalModel
    {
        public int id_nomina {  get; set; }

        [DisplayName("Periodo")]
        [Required(ErrorMessage = "Seleccione el dia de la semana correctamente")]
        public DateTime periodo_inicial { get; set; }
        public DateTime periodo_final {  get; set; }
    }
}
