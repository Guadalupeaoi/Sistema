using Microsoft.Data.SqlClient;
using Sistema.Entidad;
using Sistema.Models;
using Sistema.SistemaDL.Propiedades;
using System.Data;

namespace Sistema.SistemaBL.Propiedades
{
    public class observacionesBL
    {
        private readonly ConexionDB  _conexionDB;

        public observacionesBL(IConfiguration configuration)
        {
            _conexionDB = new ConexionDB(configuration);
        }

        public observacionesBL()
        {
        }

        public List<observacionesEL> ConsultaObservaciones(int id_observaciones)
        {
            List<observacionesEL> obList = new List<observacionesEL>();
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter { Value = id_observaciones, ParameterName = "@id_observaciones" });

            DataTable tableOb = new ObservacionesDL().ConsultaObservaciones(parametros);

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

        public List<observacionesModel> ObtenerObservacion()
        {
            var lista = new List<observacionesModel>();
            var parametros = new List<SqlParameter>();
            var conexionDB = new ConexionDB();
            DataTable dt = conexionDB.EjecutaStoredProcedureResultSet("dbo.ConsultaObservaciones", parametros);

            if (dt != null)
            {
                foreach (DataRow rw in dt.Rows)
                {
                    var item = new observacionesModel
                    {
                        id_observaciones = Convert.ToInt32(rw["id_observaciones"]),
                        codigo = Convert.ToInt32(rw["codigo"]),
                        descripcion = rw["descripcion"].ToString()
                    };
                    lista.Add(item);
                }
            }
            return lista;
        }

        public string? ObtenerDescripcion(int codigo)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@codigo", codigo)
            };
            var conexionDB = new ConexionDB();
            DataTable dt = conexionDB.EjecutaStoredProcedureResultSet("dbo.ConsultaObservaciones", parametros);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["descripcion"].ToString();
            }
            return null;
        }
    }
}
