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
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Factories;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class OrderRepo(
        ILogger<OrderRepo> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<OrderDTO>
    {
        public async Task<int> DeleteAsync(int id)
        {
            // create the query we will run
            const string query = @"DELETE FROM Orders WHERE OrderID = @OrderID";

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand(query, connection);

                // set the variable with the id sent into this function
                command.Parameters.AddWithValue("@OrderID", id);

                // runs the query
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error Deleting Order");
                return 0;
            }
        }

        public async Task<OrderDTO?> FindByIDAsync(int id)
        {
            // Create an order var
            OrderDTO order = null;

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
                    order = new OrderDTO
                    {


                        // assign the info from the columns to each of the properties of the order
                        OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                        CustomerID = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CustomerID")),
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

        public async Task<IEnumerable<OrderDTO>> GetAllAsync()
        {
            // Create a list of orders
            var orders = new List<OrderDTO>();

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
                    var order = new OrderDTO
                    {
                        // assign the info from the columns to each of the properties of the order
                        OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                        CustomerID = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CustomerID")),
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

        public async Task<int> InsertAsync(OrderDTO DTO)
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
                command.Parameters.AddWithValue("@CustomerID", DTO.CustomerID);
                command.Parameters.AddWithValue("@OrderDate", DTO.OrderDate);
                command.Parameters.AddWithValue("@ShipAmount", DTO.ShipAmount);
                command.Parameters.AddWithValue("@TaxAmount", DTO.TaxAmount);
                command.Parameters.AddWithValue("@ShipDate", DTO.ShipDate);
                command.Parameters.AddWithValue("@ShipAddressID", DTO.ShipAddressID);
                command.Parameters.AddWithValue("@CardType", DTO.CardType);
                command.Parameters.AddWithValue("@CardNumber", DTO.CardNumber);
                command.Parameters.AddWithValue("@CardExpires", DTO.CardExpires);
                command.Parameters.AddWithValue("@BillingAddressID", DTO.BillingAddressID);

                // run the query
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new order");
                return 0;
            }
        }

        public async Task<int> UpdateAsync(int id, OrderDTO dto)
        {
            // Create an update query with variables
            const string query = @"
                UPDATE Orders
                SET CustomerID = @CustomerID, OrderDate = @OrderDate, ShipDate = @ShipDate, ShipAmount = @ShipAmount,
                    TaxAmount = @TaxAmount, ShipDate = @ShipDate, ShipAddressID = @ShipAddressID, CardType = @CardType,
                    CardNumber = @CardNumber, CardExpires = @CardExpires, BillingAddressID = @BillingAddressID
                WHERE OrderID = @OrderID";

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand(query, connection);

                // set the variables with the entity sent into this function
                command.Parameters.AddWithValue("@OrderID", id);
                command.Parameters.AddWithValue("@CustomerID", dto.CustomerID);
                command.Parameters.AddWithValue("@OrderDate", dto.OrderDate);
                command.Parameters.AddWithValue("@ShipAmount", dto.ShipAmount);
                command.Parameters.AddWithValue("@TaxAmount", dto.TaxAmount);
                command.Parameters.AddWithValue("@ShipDate", dto.ShipDate);
                command.Parameters.AddWithValue("@ShipAddressID", dto.ShipAddressID);
                command.Parameters.AddWithValue("@CardType", dto.CardType);
                command.Parameters.AddWithValue("@CardNumber", dto.CardNumber);
                command.Parameters.AddWithValue("@CardExpires", dto.CardExpires);
                command.Parameters.AddWithValue("@BillingAddressID", dto.BillingAddressID);

                // run the query
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error updating order");
                throw;
            }
        }
    }
}
