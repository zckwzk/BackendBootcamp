using BackendBootcamp.Models;
using System.Data;
using System.Data.SqlClient;


namespace BackendBootcamp.Logics
{
    public class RealDBLogic
    {
        private static string connectionString = "Server=NDS-LPT-0327\\SQL2019;Database=batch6live;User Id=sa;Password=nawadata;";

        public static List<object> GetProductReader()
        {
            List<object> result = new List<object>();

            #region query proccess database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                #region query
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "Select * from Products";

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        object tempObject = new { pk_product_id = reader["pk_product_id"], product_name = reader["product_name"] };
                        result.Add(tempObject);
                    }

                    reader.Close();
                }
                conn.Close();
                #endregion
            }

            #endregion

            return result;

        }

        public static List<Product> GetProductAdapter()
        {
            List<Product> result = new List<Product>();

            string query = "select * from products";

            DataTable dataTable = CRUD.ExecuteQuery(query);

            foreach(DataRow row in dataTable.Rows)
            {
                Product tempData = new Product { pk_product_id = (int)row["pk_product_id"], product_name = (string) row["product_name"] };

                result.Add(tempData);
            }

            return result;
        }

        public static void InsertProduct(Product product)
        {
            string query = "insert into products (product_name) values ('" + product.product_name + "');";

            CRUD.ExecuteNonQuery(query);

        }

        public static string ContohScalar() {
            string result = (string) CRUD.ExecuteScalar("SELECT product_name from products where pk_product_id = 1");
            return result;
        }
    }
}
