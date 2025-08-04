using Microsoft.Data.SqlClient;
using System.Data;


namespace Sistema.SistemaDL.Propiedades
{
    public class cuentaDL
    {
        public DataTable InicioSesion(List<SqlParameter> parametros)
        {
            return new ConexionDB().EjecutaStoredProcedureResultSet("ssi.InicioSesion", parametros);
        }

        public int NuevoUsuario(List<SqlParameter> parametros)
        {
            return new ConexionDB().EjecutaStoredProcedureScalar("dbo.NuevoUsuario", parametros);
        }
    }
}