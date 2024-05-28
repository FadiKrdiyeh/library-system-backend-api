using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class SqlHandler
    {
        private readonly IConfiguration _configuration;

        public SqlHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<DataTable> ExecReaderAsync(string text)
        {
            string myDb1ConnectionString = _configuration["ConnectionString:library_system_db"].ToString();
            SqlConnection conn = new SqlConnection(myDb1ConnectionString);
            try
            {
                SqlCommand cmd = new SqlCommand(text, conn);
                cmd.CommandTimeout = 15 * 60;
                cmd.CommandType = CommandType.Text;

                conn.Open();

                DataTable dt = new DataTable();
                SqlDataReader da = await cmd.ExecuteReaderAsync();
                if (da.HasRows)
                {
                    dt.Load(da);
                }

                return dt;
            }
            catch (Exception ex)
            {
                //throw new Exception("Something went wrong");
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        public bool ExecCommandNonQuery(string queryStr, SqlParameter[] parameters)
        {
            string myDb1ConnectionString = _configuration["ConnectionString:library_system_db"].ToString();
            SqlConnection conn = new SqlConnection(myDb1ConnectionString);
            try
            {
                SqlCommand cmd = new SqlCommand(queryStr, conn);
                cmd.CommandTimeout = 15 * 60;
                cmd.CommandType = CommandType.Text;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                conn.Open();

                return Convert.ToBoolean(cmd.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                //throw new Exception("Something went wrong");
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
        public async Task<object> ExecCommandNonQueryAsync(string queryStr, SqlParameter[] parameters, bool returnId = false)
        {
            string myDb1ConnectionString = _configuration["ConnectionString:library_system_db"].ToString();
            SqlConnection conn = new SqlConnection(myDb1ConnectionString);
            try
            {
                SqlCommand cmd = new SqlCommand(queryStr, conn);
                cmd.CommandTimeout = 15 * 60;
                cmd.CommandType = CommandType.Text;
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                
                conn.Open();

                if (returnId)
                {
                    var insertedId = await cmd.ExecuteScalarAsync();
                    return insertedId;
                }
                else
                {
                    var result = await cmd.ExecuteNonQueryAsync();
                    return Convert.ToBoolean(result);
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("Something went wrong");
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
