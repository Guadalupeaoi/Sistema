using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using zkemkeeper;

namespace Sistema.SistemaDL.Propiedades
{
    public class ConexionDB

    {
        
        private readonly string _connectionString;

        public ConexionDB()

        {
            var builer = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }

        public ConexionDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ASISTEM");   
        }

        public DataTable EjecutaStoredProcedureResultSet(string storedProcedure, List<SqlParameter>parameters)
        {
            DataTable result = new DataTable();

            try
            {
                SqlConnection conn = new SqlConnection(_connectionString);
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
                SqlConnection conn = new SqlConnection(_connectionString);
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
                SqlConnection conn = new SqlConnection(_connectionString);

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

