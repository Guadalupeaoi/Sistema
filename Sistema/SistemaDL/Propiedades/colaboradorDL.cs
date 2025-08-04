using Microsoft.Data.SqlClient;
using System.Data;

namespace Sistema.SistemaDL.Propiedades
{
    public class colaboradorDL
    {
        public DataTable ConsultaNombre(List<SqlParameter> parametros)
        {
            return new ConexionDB().EjecutaStoredProcedureResultSet("dbo.ConsultaNombre", parametros);
        }

        public int NuevoColaborador(List<SqlParameter> parametros)
        {
            return new ConexionDB().EjecutaStoredProcedureScalar("dbo.NuevoColaborador", parametros);
        }

        public int ActualizaColaborador(List<SqlParameter> parametros)
        {
            return new ConexionDB().EjecutaStoredProcedureScalar("dbo.ActualizaColaborador", parametros);
        }


        
    }
}
