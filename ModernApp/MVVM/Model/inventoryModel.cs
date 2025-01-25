using System;
using System.Collections.Generic;
using System.Data;

using MySql.Data.MySqlClient;

namespace ModernApp.MVVM.Model
{
    public class inventoryModel
    {
        private readonly DBconnection _dbConnection;

        public inventoryModel()
        {
            _dbConnection = new DBconnection();
        }

        public void SaveItem(string itemId, string itemName, int qty, string type, string model, string special, string supplierName, string address, string email, string number, byte[] image)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Inventory (itemId, itemName, qty, type, model, special, supplierName, address, email, number, itemImage) VALUES (@itemId, @itemName, @qty, @type, @model, @special, @supplierName, @address, @email, @number, @ItemImage)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@itemId", itemId);
                    cmd.Parameters.AddWithValue("@itemName", itemName);
                    cmd.Parameters.AddWithValue("@qty", qty);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@model", model);
                    cmd.Parameters.AddWithValue("@special", special);
                    cmd.Parameters.AddWithValue("@supplierName", supplierName);
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@number", number);
                    cmd.Parameters.AddWithValue("@itemImage", image);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public InventoryItem FetchItemById(string itemId)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Inventory WHERE itemId = @itemId";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@itemId", itemId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new InventoryItem
                            {
                                itemId = reader["itemId"].ToString(),
                                itemName = reader["itemName"].ToString(),
                                qty = int.Parse(reader["qty"].ToString()),
                                Type = reader["type"].ToString(),
                                Model = reader["model"].ToString(),
                                Special = reader["special"].ToString(),
                                supplierName = reader["supplierName"].ToString(),
                                address = reader["address"].ToString(),
                                email = reader["email"].ToString(),
                                number = reader["number"].ToString(),
                                itemImage = reader["itemImage"] as byte[]
                            };
                        }
                    }
                }
            }

            return null; // Item not found
        }
        public void UpdateItemInDatabase(string itemId, string itemName, int qty, string type, string model, string special, string supplierName, string address, string email, string number, byte[] imageData)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                string query = "UPDATE Inventory SET itemName = @itemName, qty = @qty, Type = @Type, Model = @Model, Special = @Special, " +
                               "SupplierName = @SupplierName, Address = @Address, Email = @Email, number = @number, itemImage = @itemImage WHERE itemID = @itemID";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@itemID", itemId);
                    cmd.Parameters.AddWithValue("@itemName", itemName);
                    cmd.Parameters.AddWithValue("@qty", qty);
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.Parameters.AddWithValue("@Model", model);
                    cmd.Parameters.AddWithValue("@Special", special);
                    cmd.Parameters.AddWithValue("@SupplierName", supplierName);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@number", number);
                    cmd.Parameters.AddWithValue("@itemImage", imageData);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteItemFromDatabase(string itemId)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Inventory WHERE itemID = @itemID";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@itemID", itemId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("No item found with the given ID.");
                    }
                }
            }
        }

    }
    public class InventoryItem
    {
        public string itemId { get; set; }
        public string itemName { get; set; }
        public int qty { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public string Special { get; set; }
        public string supplierName { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string number { get; set; }
        public byte[] itemImage { get; set; }
    }

}

