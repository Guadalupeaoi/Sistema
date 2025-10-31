using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Entidad
{
    public class asistenciaEL
    {
        public int Id_asistencia {  get; set; }
        public string? colaborador { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora_entrada { get; set; }
        public TimeSpan Hora_salida { get; set;}
        public string? Estado { get; set; }
        public DateTime Dias_labores { get; set; }
    }
}
