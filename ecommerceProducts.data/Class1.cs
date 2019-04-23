using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ecommerceProducts.data
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int CategoryId { get; set; }

    }

    public class Category
    {
        public Category()
        {
            Products = new List<Product>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }

    public class CartItem
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CheckoutItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ShoppingManager
    {
        private string _connectionString;

        public ShoppingManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddProduct(Product p)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Products VALUES (@Name, @Description, @Price, @CategoryId, @Image)";
            cmd.Parameters.AddWithValue("@Name", p.Name);
            cmd.Parameters.AddWithValue("@Description", p.Description);
            cmd.Parameters.AddWithValue("@Price", p.Price);
            cmd.Parameters.AddWithValue("@CategoryId", p.CategoryId);
            cmd.Parameters.AddWithValue("@Image", p.Image);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public void AddCategory(Category c)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Categories VALUES (@Name)";
            cmd.Parameters.AddWithValue("@Name", c.Name);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public List<Category> GetAllCategories()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Categories";
            List<Category> categories = new List<Category>();
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(new Category
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Products = GetProductsForCategory((int)reader["Id"])
                });
            }
            conn.Close();
            conn.Dispose();
            return categories;
        }

        public int GetTopCategory()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT TOP 1 Id FROM Categories";
            conn.Open();
            int id = (int)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return id;
        }

        public List<Product> GetProductsForCategory(int? id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Products WHERE CategoryId=@id";
            if (id == null)
            {
                cmd.Parameters.AddWithValue("@id", GetTopCategory());
            }
            else
            {
                cmd.Parameters.AddWithValue("@id", id);
            }
            List<Product> products = new List<Product>();
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Description = (string)reader["Description"],
                    Price = (decimal)reader["Price"],
                    CategoryId = (int)reader["CategoryId"],
                    Image = (string)reader["Image"]
                });
            }
            conn.Close();
            conn.Dispose();
            return products;
        }

        public Product GetProduct(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Products WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            Product p = new Product();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                p.Id = (int)reader["Id"];
                p.Name = (string)reader["Name"];
                p.Description = (string)reader["Description"];
                p.Price = (decimal)reader["Price"];
                p.CategoryId = (int)reader["CategoryId"];
                p.Image = (string)reader["Image"];
            }
            conn.Close();
            conn.Dispose();
            return p;
        }

        public void AddToCart(CartItem c)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO ShoppingCartItems VALUES (@CartId, @ProductId, @Quantity)";
            cmd.Parameters.AddWithValue("@CartId", c.CartId);
            cmd.Parameters.AddWithValue("@ProductId", c.ProductId);
            cmd.Parameters.AddWithValue("@Quantity", c.Quantity);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public int AddCart()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO ShoppingCarts VALUES (@date) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            conn.Open();
            int id = (int)(decimal)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return id;

        }

        public List<Product> GetProductsForCart(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM ShoppingCartItems s JOIN Products p ON p.Id = s.ProductId WHERE s.ShoppingCartId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            List<Product> products = new List<Product>();
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Description = (string)reader["Description"],
                    Price = (decimal)reader["Price"],
                    CategoryId = (int)reader["CategoryId"],
                    Image = (string)reader["Image"]
                });
            }
            conn.Close();
            conn.Dispose();
            return products;
        }

        public int GetQuantityForProduct(int cartId, int productId)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Quantity FROM ShoppingCartItems WHERE ShoppingCartId=@cartId AND ProductId=@productId";
            cmd.Parameters.AddWithValue("@cartId", cartId);
            cmd.Parameters.AddWithValue("@productId", productId);
            conn.Open();
            int quantity = (int)cmd.ExecuteScalar();           
            conn.Close();
            conn.Dispose();
            return quantity;
        }

        public void UpdateCart(CartItem c)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE ShoppingCartItems SET Quantity = @quantity WHERE ShoppingCartId=@cartId AND ProductId=@productId";
            cmd.Parameters.AddWithValue("@cartId", c.CartId);
            cmd.Parameters.AddWithValue("@productId", c.ProductId);
            cmd.Parameters.AddWithValue("@quantity", c.Quantity);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public void DeleteItem(CartItem c)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM ShoppingCartItems WHERE ShoppingCartId=@cartId AND ProductId=@productId";
            cmd.Parameters.AddWithValue("@cartId", c.CartId);
            cmd.Parameters.AddWithValue("@productId", c.ProductId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public void AddUser(User user)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users VALUES (@name, @email, @password)";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@password", HashPassword(user.Password));
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public User GetUserByEmail(User user)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users WHERE Email=@email";
            cmd.Parameters.AddWithValue("@email", user.Email);
            conn.Open();
            User u = new User();
            SqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                u.Name = (string)reader["name"];
                u.Email = (string)reader["email"];
                u.Password = (string)reader["password"];
            }
            conn.Close();
            conn.Dispose();
            return u;
        }

        public string HashPassword(string password)
        {
           return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Match(string input, string passwordHash)
        {
          return BCrypt.Net.BCrypt.Verify(input, passwordHash);
        }

    }
}
