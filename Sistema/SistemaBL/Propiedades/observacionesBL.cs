using Microsoft.Data.SqlClient;
using Sistema.Entidad;
using Sistema.SistemaDL.Propiedades;
using System.Data;

namespace Sistema.SistemaBL.Propiedades
{
    public class observacionesBL
    {
        public List<observacionesEL> ConsultaObservaciones(int id_observaciones)
        {
            List<observacionesEL> obList = new List<observacionesEL> ();
            List<SqlParameter> parametros = new List<SqlParameter> ();

            parametros.Add(new SqlParameter { Value = id_observaciones, ParameterName = "@id_observaciones"  });

            DataTable tableOb = new ObservacionesDL().ConsultaObservaciones (parametros);

            foreach (DataRow row in tableOb.Rows)
            {
                int evaluaEntero;
                observacionesEL itemOb = new observacionesEL();

                itemOb.id_observaciones = Int32.TryParse(row["Id_asistencia"].ToString(), out evaluaEntero) ? evaluaEntero : 0;
                itemOb.codigo = Int32.TryParse(row["codigo"].ToString(), out evaluaEntero) ? evaluaEntero : 0;
                itemOb.descripcion = !string.IsNullOrEmpty(row["descripcion"].ToString()) ? row["Estado"].ToString() : "N/A";
                
                obList.Add(itemOb);
                
            }

            return obList;
        }
    }
}
