using Microsoft.Data.SqlClient;
using Sistema.SistemaEL.Propiedades;
using Sistema.SistemaDL.Propiedades;
using System.Data;

namespace Sistema.SistemaBL.Propiedades
{
    public class cuentaBL
    {
        public List<cuentaEL> InicioSesion(string nombreUsuario, string password)
        {
            List<cuentaEL> usuarioList = new List<cuentaEL>();
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter { Value = nombreUsuario, ParameterName = "@NombreUsuario" });
            parametros.Add(new SqlParameter { Value = password, ParameterName = "@Password" });

            DataTable tableUsuario = new cuentaDL().InicioSesion(parametros);

            foreach (DataRow row in tableUsuario.Rows)
            {

                int evaluaEntero;
                cuentaEL itemUsuario = new cuentaEL();

                itemUsuario.idCuenta = Int32.TryParse(row["Id_asistencia"].ToString(), out evaluaEntero) ? evaluaEntero : 0;
                itemUsuario.nombreUsuario = !string.IsNullOrEmpty(row["nombreUsuario"].ToString()) ? row["nombreUsuario"].ToString() : "N/A";
                itemUsuario.correo = !string.IsNullOrEmpty(row["correo"].ToString()) ? row["correo"].ToString() : "N/A";
                itemUsuario.idRolUsuario = Int32.TryParse(row["IdRolUsuario"].ToString(), out evaluaEntero) ? evaluaEntero : 0;

                usuarioList.Add(itemUsuario);
            }
            return usuarioList;
        }

        public int NuevoUsuario(cuentaEL entidad)
        {
            int result;

            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter { Value = entidad.idCuenta, ParameterName = "@idcuenta" });
            parametros.Add(new SqlParameter { Value = entidad.nombreUsuario, ParameterName = "@nombreUsuario" });
            parametros.Add(new SqlParameter { Value = entidad.correo, ParameterName = "@correo" });
            parametros.Add(new SqlParameter { Value = entidad.idRolUsuario, ParameterName = "@idRolUsuario" });

            result = new cuentaDL().NuevoUsuario(parametros);

            return result;
        }
    }
}
