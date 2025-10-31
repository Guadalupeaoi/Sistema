using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Sistema.Models
{
  
        public class asistenciaModel
        {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_asistencia { get; set; }

        [DisplayName("Fecha")]
        [Required(ErrorMessage = "Favor de ingresar la fecha indicada")]
        public DateTime fecha { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        // FK hacia Colaborador
        public int? num_empleado { get; set; }

        [ForeignKey("num_empleado")]
        public ColaboradorModel? colaborador { get; set; }
     

        public DateTime? hora_entrada { get; set; }

        public DateTime? hora_salida { get; set; }

        public string? estado { get; set; }
        
        [DisplayName("Dias laborales")]
        public DateTime dias_laborales { get; set; }

        public string num_empleado_reloj { get; set; }



    }



}

