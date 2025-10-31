using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Sistema.Entidad;
using Sistema.SistemaDL;
using Sistema.SistemaDL.Propiedades;
using System.Data;

namespace Sistema.SistemaBL.Propiedades
{
    public class colaboradorBL
    {
        public List<colaboradorEl> ConsultaNombre(string Nombre, int numempleado, int v, string Apellido, string apellidos)
        {
            List<colaboradorEl> colaList = new List<colaboradorEl>();
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter { Value = Nombre, ParameterName = "@Nombre" });
            parametros.Add(new SqlParameter { Value = Apellido, ParameterName = "@Apellido" });

            DataTable tablaNombre = new colaboradorDL().ConsultaNombre(parametros);

            foreach (DataRow row in tablaNombre.Rows)
            {
                int evaluaEntero;
                colaboradorEl itemColab = new colaboradorEl();

                itemColab.Num_empleado = Int32.TryParse(row["Num_empleado"].ToString(), out evaluaEntero) ? Convert.ToInt32(row["Num_empleado"]) : 0;
                itemColab.nombre = !string.IsNullOrEmpty(row["nombre"].ToString()) ? row["nombre"].ToString() : "";
                itemColab.apellido = !string.IsNullOrEmpty(row["apellido"].ToString()) ? row["apellido"].ToString() : "";
                itemColab.area = !string.IsNullOrEmpty(row["area"].ToString()) ? row["area"].ToString() : "";

                colaList.Add(itemColab);
            }

            return colaList;
        }

        public int NuevoColaborador(colaboradorEl entidad)
        {
            int result;

            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter { Value = entidad.Num_empleado, ParameterName = "@Num_empleado" });
            parametros.Add(new SqlParameter { Value = entidad.nombre, ParameterName = "@nombre" });
            parametros.Add(new SqlParameter { Value = entidad.apellido, ParameterName = "@apellido" });
            parametros.Add(new SqlParameter { Value = entidad.area, ParameterName = "@area" });

            result = new colaboradorDL().NuevoColaborador(parametros);

            return result;
        }

        public int ActualizaColaborador(colaboradorEl endtidad)
        {
            int result;

            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(new SqlParameter { Value = endtidad.Num_empleado, ParameterName = "@Num_empleado" });
            parametros.Add(new SqlParameter { Value = endtidad.nombre, ParameterName = "@nombre" });
            parametros.Add(new SqlParameter { Value = endtidad.apellido, ParameterName = "@apellido" });
            parametros.Add(new SqlParameter { Value = endtidad.area, ParameterName = "@area" });

            result = new colaboradorDL().ActualizaColaborador(parametros);
            return result;  
        }

        internal List<colaboradorEl> ConsultaNombre(string searching, int numempleado, string nombres, string apellidos, int v)
        {
            throw new NotImplementedException();
        }

        internal object ConsultaNombre(string empty, int v1, int v2, int v3, int numEmpleado)
        {
            throw new NotImplementedException();
        }

        internal colaboradorEl ConsultaNombre(string empty, int v1, int v2, int v3, List<SelectListItem>? nombres)
        {
            throw new NotImplementedException();
        }
    }
}