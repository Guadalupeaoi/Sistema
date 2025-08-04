using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Sistema.SistemaDL.Propiedades
{
    public class ConexionDB
    {
        private string ConnectionString { get; set; }

        public ConexionDB()
        {
            this.ConnectionString = IConfiguration.ConnectionStrings["ASISTEM"].ToString();
        }

        /// <summary>
        /// Ejecución de procedimientos almacenados, con base en sus parámetros
        /// </summary>
        /// <param name="storedProcedure">Contiene el nombre del procedimiento almacenado</param>
        /// <param name="parameters">Lista de parámetros que requiere el SP</param>
        /// <returns>Result Set</returns>
        public DataTable EjecutaStoredProcedureResultSet(string storedProcedure, List<SqlParameter>parameters)
        {
            DataTable result = new DataTable();

            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                SqlCommand cmd = new SqlCommand(storedProcedure, conn);

                foreach (SqlParameter param in parameters)
                    cmd.Parameters.Add(param);

                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(result);
                }
            }
            catch (Exception ex)
            {
                return  null;
            }

            return result;
        }

        /// <summary>
        /// Ejecución de procedimientos almacenados, con base en sus parámetros
        /// </summary>
        /// <param name="storedProcedure">Contiene el nombre del procedimiento almacenado</param>
        /// <param name="parameters">Lista de parámetros que requiere el SP</param>
        /// <returns>Valor Scalar</returns>
        public int EjecutaStoredProcedureScalar(string storedProcedure, List<SqlParameter> parameters)
        {
            int result;

            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                SqlCommand cmd = new SqlCommand(storedProcedure, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter param in parameters)
                    cmd.Parameters.Add(param);

                conn.Open();
                object resultObj = cmd.ExecuteScalar();
                resultObj = (resultObj == DBNull.Value) ? null : resultObj;
                result = Convert.ToInt32(resultObj);
            }
            catch (Exception ex)
            {
                result = 0;
            }

            return result;
        }

        public int EjecutaBulkCopy(DataTable tableDestination)
        {
            int result;

            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    conn.Open();
                    bulkCopy.DestinationTableName = tableDestination.TableName;
                    bulkCopy.WriteToServer(tableDestination);
                }

                result = 1;
            }
            catch (Exception ex)
            {
                result = 0;
            }

            return result;
        }
    }

}