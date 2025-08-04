using Microsoft.Data.SqlClient;
using Sistema.Entidad;
using Sistema.SistemaDL.Propiedades;
using System.Data;

namespace Sistema.SistemaBL.Propiedades
{
    public class asistenciaBL
    {
        public List<asistenciaEL> ConsultaAsistencia(string searchString, int Id_asistencia)
        {
            List<asistenciaEL> asisList = new List<asistenciaEL>();
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter { Value = Id_asistencia, ParameterName = "@Id_asistencia" });
       

            DataTable tableAsis = new asistenciaDL().ConsultaAsistencia(parametros);

            foreach (DataRow row in tableAsis.Rows)
            {
                int evaluaEntero;
                DateTime evaluaFecha;
                TimeSpan evaluaHora;
                asistenciaEL itemAsistencia = new asistenciaEL();

                itemAsistencia.Id_asistencia = Int32.TryParse(row["Id_asistencia"].ToString(), out evaluaEntero) ? evaluaEntero : 0;
                itemAsistencia.Fecha = DateTime.TryParse(row["fecha"].ToString(), out evaluaFecha) ? evaluaFecha : new DateTime(2000, 01, 01);
                itemAsistencia.Hora_entrada = TimeSpan.TryParse(row["Hora_entrada"].ToString(), out evaluaHora) ? evaluaHora : TimeSpan.Zero;
                itemAsistencia.Hora_salida = TimeSpan.TryParse(row["Hora_salida"].ToString(), out evaluaHora) ? evaluaHora : TimeSpan.Zero;
                itemAsistencia.Estado = !string.IsNullOrEmpty(row["Estado"].ToString()) ? row["Estado"].ToString() : "N/A";
                itemAsistencia.Dias_labores = DateTime.TryParse(row["Dias_laborales"].ToString(), out evaluaFecha) ? evaluaFecha: new DateTime(2000, 01, 01);

                asisList.Add(itemAsistencia);

            }

            return asisList;
        }

       
         public List<asistenciaEL> ConsultaDias(string searchString, DateTime Fecha, DateTime Dias_laborales)
        {
            List<asistenciaEL> asisFecha = new List<asistenciaEL>();
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter { Value = Fecha, ParameterName = "@Fecha" });
            parametros.Add(new SqlParameter { Value = Dias_laborales, ParameterName = "@Dias_laborales" });

            DataTable tableFecha = new asistenciaDL().ConsultaDias(parametros);

            foreach (DataRow row in tableFecha.Rows){
                DateTime evaluaFecha;
                asistenciaEL itemfecha = new asistenciaEL();

                itemfecha.Fecha = DateTime.TryParse(row["Fecha"].ToString(), out evaluaFecha) ? evaluaFecha : new DateTime(2000, 01, 01);
                itemfecha.Dias_labores = DateTime.TryParse(row["Dias_laborales"].ToString(), out evaluaFecha) ? evaluaFecha : new DateTime(2000, 01, 01);
                asisFecha.Add(itemfecha);
            }

            return asisFecha;
        }
    }
}
