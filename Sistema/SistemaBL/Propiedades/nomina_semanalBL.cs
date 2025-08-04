using Microsoft.Data.SqlClient;
using Sistema.Entidad;
using Sistema.SistemaDL.Propiedades;
using System.Data;

namespace Sistema.SistemaBL.Propiedades
{
    public class nomina_semanalBL
    {
        public List<nominaSemanalEL> ConsultaSemana(int id_nomina)
        {
            List<nominaSemanalEL> nominaList = new List<nominaSemanalEL>();
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter { Value = id_nomina, ParameterName = "@id_nomina" });

            DataTable tablenomina = new nomina_semanalDL().ConsultaSemana(parametros);

            foreach (DataRow row in tablenomina.Rows)
            {
                int evaluaEntero;
                DateTime evaluaFecha;
                nominaSemanalEL itemnomi = new nominaSemanalEL();

                itemnomi.id_nomina = Int32.TryParse(row["Id_nomina"].ToString(), out evaluaEntero) ? evaluaEntero: 0;
                itemnomi.periodo_inicial = DateTime.TryParse(row["periodo_inicial"].ToString(), out evaluaFecha) ? evaluaFecha : new DateTime(2000, 01, 01);
                itemnomi.periodo_final = DateTime.TryParse(row["periodo_final"].ToString(), out evaluaFecha) ? evaluaFecha : new DateTime(2000, 01, 01);
              
                nominaList.Add(itemnomi);
            }

            return nominaList;

        }

    }
}
