using BackendBootcamp.Models;
using MySqlX.XDevAPI.Common;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;


namespace BackendBootcamp.Logics
{
    public class RealDBLogic
    {
        //private static string connectionString = "Server=NDS-LPT-0327\\SQL2019;Database=batch6live;User Id=sa;Password=nawadata;";
        private static string connectionString = "";

        public static void GetConfiguration(IConfiguration configuration) {
            connectionString = configuration["ConnectionStrings:Default"];
        }

        public static List<object> GetProductReader(string name)
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
                    cmd.CommandText = "Select * from Products where product_name like @name";


                    //Set SqlParameter
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@name";
                    param.SqlDbType = SqlDbType.VarChar;
                    param.Value = "%" + name + "%";
                    //Add SqlParameter to SqlCommand
                    cmd.Parameters.Add(param);


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

        public static List<Product> GetProductAdapter(string name = "")
        {
            List<Product> result = new List<Product>();

            string query = "select * from products";

            if (!String.IsNullOrEmpty(name))
            {
                query += " where product_name like @name";
            }

            SqlParameter[] sqlparams = new SqlParameter[]
            {
                new SqlParameter("@name",SqlDbType.VarChar){Value = "%" + (name ?? "") + "%" }
            };

            DataTable dataTable = CRUD.ExecuteQuery(query,sqlparams);

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

        public static string InsertUserWithProduct(UserWithProducs userProduct)
        {

            #region query proccess database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                #region query
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.Transaction = conn.BeginTransaction();
                    try
                    {
                        cmd.CommandText = "INSERT INTO users ([name]) values (@name);select SCOPE_IDENTITY();";
                        
                        cmd.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar) { Value = userProduct.name });

                        decimal pk_user_id = (decimal) cmd.ExecuteScalar();

                        cmd.Parameters.Clear();

                        foreach(Product product in userProduct.products)
                        {
                            cmd.CommandText = "INSERT INTO Products (product_name, fk_users_id) values (@prodname, @fkuser)";
                            cmd.Parameters.Add(new SqlParameter("@prodname", SqlDbType.VarChar) { Value = product.product_name ?? "" });
                            cmd.Parameters.Add(new SqlParameter("@fkuser", SqlDbType.Int) { Value = pk_user_id });

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        cmd.Transaction.Commit();
                        conn.Close();
                        return "Success";
                    }catch (Exception ex)
                    {
                        cmd.Transaction.Rollback();
                        conn.Close();
                        throw ex;
                    }
                }
                #endregion
            }

            #endregion
        }
    }
}
