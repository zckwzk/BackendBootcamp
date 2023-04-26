using System.Data;
using System.Data.SqlClient;

namespace BackendBootcamp.Models
{
    public class CRUD
    {
        //private static string connectionString = "Server=NDS-LPT-0327\\SQL2019;Database=batch6live;User Id=sa;Password=nawadata;";
        private static string connectionString = "";

        public static void GetConfiguration(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:Default"];
        }

        public static DataTable ExecuteQuery(string sql, SqlParameter[] sqlParameters = null)
        {
            DataTable result = new DataTable();

            #region query proccess database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                #region query
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if(sqlParameters != null) { cmd.Parameters.AddRange(sqlParameters); }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                conn.Close();
                #endregion
            }
            #endregion

            return result;
        }

        #region ExecuteScalar
        /// <summary>
        /// ExecuteScalar untuk menjalankan query yang hanya return tepat 1 data
        /// </summary>
        public static object ExecuteScalar(string query, SqlParameter[] sqlParameters = null)
        {
            object result = null;

            // begin connection
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                #region query process to database
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (sqlParameters != null) { cmd.Parameters.AddRange(sqlParameters); }

                    result = cmd.ExecuteScalar(); // ExecuteScalar untuk query yang return tepat 1 data saja
                }
                #endregion

                // close connection
                con.Close();
            }

            return result;
        }
        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// ExecuteNonQuery untuk menjalankan query yang tidak return apa-apa
        /// </summary>
        public static int ExecuteNonQuery(string query, SqlParameter[] sqlParameters = null)
        {
            int result = 0;

            // begin connection
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                #region query process to database
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (sqlParameters != null) { cmd.Parameters.AddRange(sqlParameters); }

                    result = cmd.ExecuteNonQuery(); // ExecuteNonQuery untuk query yang tidak return apa-apa
                }
                #endregion

                // close connection
                con.Close();
            }

            return result;
        }
        #endregion

    }
}
