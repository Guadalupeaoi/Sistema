using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Sistema.SistemaDL.Propiedades
{
    public class asistenciaDL
    {
      
        public DataTable ConsultaAsistencia(List<SqlParameter> parametros)

        {
            return new ConexionDB().EjecutaStoredProcedureResultSet("dbo.ConsultaAsistencia", parametros);
        }

        public DataTable ConsultaDias(List<SqlParameter> parametros)
        {
            return new ConexionDB().EjecutaStoredProcedureResultSet("db.ConsultaDias", parametros);
        }
    }

}

