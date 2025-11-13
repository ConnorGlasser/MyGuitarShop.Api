using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Factories;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class OrderRepo(
        ILogger<OrderRepo> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<OrderEntity>
    {
        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderEntity?> FindByIDAsync(int id)
        {
            // Create an order var
            OrderEntity order = null;

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand("SELECT * FROM Orders WHERE OrderID = @OrderID", connection);

                // set the variable @OrderID with the id sent into this function
                command.Parameters.AddWithValue("@OrderID", id);

                // creates an SQL Data reader that will read data from our sql tables
                await using var reader = await command.ExecuteReaderAsync();

                // if the orderID isn't null
                if (await reader.ReadAsync())
                {
                    order = new OrderEntity
                    {
                        // assign the info from the columns to each of the properties of the order
                        OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                        CustomerID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                        ShipAmount = reader.GetDecimal(reader.GetOrdinal("ShipAmount")),
                        TaxAmount = reader.GetDecimal(reader.GetOrdinal("TaxAmount")),
                        ShipDate = reader.IsDBNull(reader.GetOrdinal("ShipDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ShipDate")),
                        ShipAddressID = reader.GetInt32(reader.GetOrdinal("ShipAddressID")),
                        CardType = reader.GetString(reader.GetOrdinal("CardType")),
                        CardNumber = reader.GetString(reader.GetOrdinal("CardNumber")),
                        CardExpires = reader.GetString(reader.GetOrdinal("CardExpires")),
                        BillingAddressID = reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error finding order {id} by ID");
            }

            // return the order, or null if an error occured
            return order;
        }

        public async Task<IEnumerable<OrderEntity>> GetAllAsync()
        {
            // Create a list of orders
            var orders = new List<OrderEntity>();

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand("SELECT * FROM Orders", connection);

                // creates an SQL Data reader that will read data from our sql tables
                await using var reader = await command.ExecuteReaderAsync();

                // Keep reading through every row 
                while (await reader.ReadAsync())
                {
                    // create a new order var
                    var order = new OrderEntity
                    {
                        // assign the info from the columns to each of the properties of the order
                        OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                        CustomerID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                        ShipAmount = reader.GetDecimal(reader.GetOrdinal("ShipAmount")),
                        TaxAmount = reader.GetDecimal(reader.GetOrdinal("TaxAmount")),
                        ShipDate = reader.IsDBNull(reader.GetOrdinal("ShipDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ShipDate")),
                        ShipAddressID = reader.GetInt32(reader.GetOrdinal("ShipAddressID")),
                        CardType = reader.GetString(reader.GetOrdinal("CardType")),
                        CardNumber = reader.GetString(reader.GetOrdinal("CardNumber")),
                        CardExpires = reader.GetString(reader.GetOrdinal("CardExpires")),
                        BillingAddressID = reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
                    };
                    // add this order to the list
                    orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving order list");
            }

            // Return the list of orders
            // if there is an error, it will return a semi-complete list (everything up to the error)
            return orders;
        }

        public async Task<int> InsertAsync(OrderEntity entity)
        {
            // Create an insert query with variables
            const string query = @"
                INSERT INTO Orders (CustomerID, OrderDate, ShipAmount, TaxAmount, ShipDate, ShipAddressID, CardType, CardNumber, CardExpires, BillingAddressID) 
                VALUES (@CustomerID, @OrderDate, @ShipAmount, @TaxAmount, @ShipDate, @ShipAddressID, @CardType, @CardNumber, @CardExpires, @BillingAddressID);";

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand(query, connection);

                // set the variable with the entity sent into this function
                command.Parameters.AddWithValue("@CustomerID", entity.CustomerID);
                command.Parameters.AddWithValue("@OrderDate", entity.OrderDate);
                command.Parameters.AddWithValue("@ShipAmount", entity.ShipAmount);
                command.Parameters.AddWithValue("@TaxAmount", entity.TaxAmount);
                command.Parameters.AddWithValue("@ShipDate", entity.ShipDate);
                command.Parameters.AddWithValue("@ShipAddressID", entity.ShipAddressID);
                command.Parameters.AddWithValue("@CardType", entity.CardType);
                command.Parameters.AddWithValue("@CardNumber", entity.CardNumber);
                command.Parameters.AddWithValue("@CardExpires", entity.CardExpires);
                command.Parameters.AddWithValue("@BillingAddressID", entity.BillingAddressID);

                // run the query
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new order");
                return 0;
            }
        }

        public async Task<int> UpdateAsync(int id, OrderEntity DTO)
        {
            // Create an update query with variables
            const string query = @"
                UPDATE Orders
                SET CustomerID = @CustomerID, ShipDate = @ShipDate, ShipAddressID = @ShipAddressID
                WHERE OrderID = @OrderID";

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand(query, connection);

                // set the variables with the entity sent into this function
                command.Parameters.AddWithValue("@OrderID", id);
                command.Parameters.AddWithValue("@CustomerID", DTO.CustomerID);
                command.Parameters.AddWithValue("@ShipDate", DTO.ShipDate);
                command.Parameters.AddWithValue("@ShipAddressID", DTO.ShipAddressID);

                // run the query
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error updating order");
                return 0;
            }
        }
    }
}
