namespace BackendBootcamp.Models
{
    public class Product
    {
        public int pk_product_id { get; set; }
        public string product_name { get; set; }

        public int fk_user_id { get; set; }
    }

    public class User
    {
        public int pk_user_id { get; set; }

        public string name { get; set; }
    }

    public class UserWithProducs : User
    {
        public List<Product> products { get; set; }
    }


}
