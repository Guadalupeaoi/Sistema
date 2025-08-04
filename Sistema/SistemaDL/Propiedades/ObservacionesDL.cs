using Microsoft.Data.SqlClient;
using System.Data;

namespace Sistema.SistemaDL.Propiedades
{
    public class ObservacionesDL
    {
        public DataTable ConsultaObservaciones(List<SqlParameter> parametros)
        {
            return new ConexionDB().EjecutaStoredProcedureResultSet("dbo.ConsultaObservaciones", parametros);
        }
    }
}
