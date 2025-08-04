using Microsoft.Data.SqlClient;
using System.Data;

namespace Sistema.SistemaDL.Propiedades
{
    public class nomina_semanalDL
    {
        public DataTable ConsultaSemana(List<SqlParameter> parametros)
        {
            return new ConexionDB().EjecutaStoredProcedureResultSet("dbo.ConsultaSemana", parametros);
        }

      
    }
}
