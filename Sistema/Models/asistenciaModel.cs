using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Sistema.Models
{
  
        public class asistenciaModel
        {

        public int id_asistencia { get; set; }

        [DisplayName("Fecha")]
        [Required(ErrorMessage = "Favor de ingresar la fecha indicada")]
        public DateTime fecha { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public TimeSpan hora_entrada { get; set; }

        public TimeSpan hora_salida { get; set; }

        public string? estado { get; set; }
        
        [DisplayName("Dias laborales")]
        public DateTime dias_laborales { get; set; }
        public List<SelectListItem>? DiasLaborales { get; set; }


        }
    
}

