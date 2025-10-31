using Microsoft.Data.SqlClient;
using Microsoft.Office.Interop.Excel;
using Sistema.Models;
using Sistema.SistemaEL.Propiedades;


namespace Sistema.SistemaDL.Propiedades
{
    public class HuellaDL
    {
        private readonly String _connectionString;

        public HuellaDL (string connectionString)
        {
            _connectionString = connectionString;
        }


        public void InsertarHuella(HuellaEl huella) {
            using (SqlConnection conexion = new SqlConnection(_connectionString))
            {
              
                string query = "INSERT INTO Huellas (IdUsuario, DedoIndex, TemplateHuella, FechaRegistro) " +
                               "VALUES (@IdUsuario, @DedoIndex, @TemplateHuella, @FechaRegistro)";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@IdUsuario", huella.IdUsuario);
                cmd.Parameters.AddWithValue("@DedoIndex", huella.DedoIndex);
                cmd.Parameters.AddWithValue("@TemplateHuella", huella.TemplateHuella);
                cmd.Parameters.AddWithValue("@FechaRegistro", huella.FechaRegistro);

                conexion.Open();

            }

        }

        public List<HuellaModelo> ConsultaHuella(int Usuario)
        {
            List<HuellaModelo> list = new List<HuellaModelo>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Huellas WHERE IdUsuario = @IdUsuario";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdUsuario", Usuario);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new HuellaModelo
                    {
                        IdHuella = Convert.ToInt32(reader["IdHuella"]),
                        IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                        DedoIndex = Convert.ToInt32(reader["DedoIndex"]),
                        TemplateHuella = reader["TemplateHuella"].ToString(),
                        FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
                    });
                }
            }
            return list;
        }

    }


}

