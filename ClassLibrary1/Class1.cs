using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ClassLibrary1
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
    }
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Amount { get; set; }
    }
    public class ShoppingCart
    {
        public int UserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public class ShoppingCartItem
    {
        public int ShoppingCartId { get; set;}
        public int ProductId { get; set; }
        public int Quantity { get; set; }

    }
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ECommerceManager
    {
        private string _connectionString;

        public ECommerceManager(string connectionString)
        {
            _connectionString = connectionString;
        }
        #region User
        public void AddUser(User user, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                                  "VALUES (@name, @email, @passwordHash)";
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public User GetByEmail(string email)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                return new User
                {
                    Email = email,
                    Name = (string)reader["Name"],
                    Id = (int)reader["Id"],
                    PasswordHash = (string)reader["PasswordHash"]
                };
            }
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isCorrectPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (isCorrectPassword)
            {
                return user;
            }

            return null;
        }
        #endregion
        #region Category
        public int AddCategory(string name)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Insert into Category" +
                    " values (@name) select scope_identity()";
                cmd.Parameters.AddWithValue("@name", name);
                conn.Open();
                return (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public IEnumerable<Category> GetCategories()
        {
            List<Category> names = new List<Category>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select * from Category";
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Category c = new Category
                    {
                        Name = (string)reader["name"],
                        Id = (int)reader["Id"],
                    };

                    names.Add(c);
                }
            }
            return names;
        }
        #endregion
        #region Product
        public void AddProduct(Product p)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Product " +
                                  "VALUES (@name, @filename, @amount,@categoryid, @userid,@description)";
                cmd.Parameters.AddWithValue("@name", p.Name);
                cmd.Parameters.AddWithValue("@filename", p.FileName);
                cmd.Parameters.AddWithValue("@amount", p.Amount);
                cmd.Parameters.AddWithValue("@categoryid", p.CategoryId);
                cmd.Parameters.AddWithValue("@userid", p.UserId);
                cmd.Parameters.AddWithValue("@description", p.Description);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public IEnumerable<Product> GetProductsForCatId(int id)
        {
            List<Product> products = new List<Product>();

            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "Select * from product where categoryid = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = (int)reader["id"],
                        Name = (string)reader["name"],
                        FileName = (string)reader["filename"],
                        Amount = (decimal)reader["amount"],
                        UserId = (int)reader["userid"],
                        CategoryId = (int)reader["categoryid"],
                        Description = (string)reader["description"],
                    });
                }

            }

            return products;

        }
        public Product GetProductForId(int id)
        {
            Product product = new Product();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "Select * from product where id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    product.Id = (int)reader["id"];
                    product.Name = (string)reader["name"];
                    product.FileName = (string)reader["filename"];
                    product.Amount = (decimal)reader["amount"];
                    product.UserId = (int)reader["userid"];
                    product.CategoryId = (int)reader["categoryid"];
                    product.Description = (string)reader["description"];
                    
                }

            }

            return product;
        }
        #endregion
        #region ShoppingCart
        public int AddShoppingCart()
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Insert into shoppingcart" +
                    " values (@datecreated) select scope_identity()";
                cmd.Parameters.AddWithValue("@datecreated", DateTime.Now);
                conn.Open();
                return (int)(decimal)cmd.ExecuteScalar();
            }
        }
        public void AddItemToShoppingCart(ShoppingCartItem item)
        {
            
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO shoppingcartitems " +
                                  "VALUES (@shoppingcartid, @productid, @quantity)";
                cmd.Parameters.AddWithValue("@shoppingcartid", item.ShoppingCartId);
                cmd.Parameters.AddWithValue("@productid", item.ProductId);
                cmd.Parameters.AddWithValue("@quantity", item.Quantity);
                 connection.Open();
                cmd.ExecuteNonQuery();

            }
        }
        public IEnumerable<ShoppingCartItem> GetShoppingCartItemsForShoppingCartId(int id)
        {
            List<ShoppingCartItem> items = new List<ShoppingCartItem>();

            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "Select * from shoppingcartitems where shoppingcartid = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new ShoppingCartItem
                    {
                        ShoppingCartId = (int)reader["shoppingcartid"],
                        ProductId = (int)reader["productid"],
                        Quantity = (int)reader["quantity"],
                    });
                }

            }

            return items;

        }
        public void DeleteItemFromShoppingCart(int scid, int pid)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "Delete shoppingcartitems " +
                                  "where shoppingcartid = @scid and productid = @pid";
                cmd.Parameters.AddWithValue("@scid", scid);
                cmd.Parameters.AddWithValue("@pid",pid);
                connection.Open();
                cmd.ExecuteNonQuery();

            }
        }
        public void UpdateItemInShoppingCart(int scid, int pid, int quantity)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "Update shoppingcartitems set quantity = @quantity " +
                                  "where shoppingcartid = @scid and productid = @pid";
                cmd.Parameters.AddWithValue("@scid", scid);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                connection.Open();
                cmd.ExecuteNonQuery();

            }
        }
        #endregion

    }
}

//On the nav bar, there should be a link that takes the user to their cart. 
//        On the cart page, display a list of all products with images that are in their cart.
//        The user should be able to update the quantity, or delete products from their cart.


