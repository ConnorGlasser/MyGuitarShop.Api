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
    public class OrderItemRepo(
        ILogger<OrderItemRepo> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<OrderItemEntity>
    {
        public async Task<IEnumerable<OrderItemEntity>> GetAllAsync()
        {
            // Create a list of orderItems
            var orderItems = new List<OrderItemEntity>();

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand("SELECT * FROM Customers", connection);

                // creates an SQL Data reader that will read data from our sql tables
                await using var reader = await command.ExecuteReaderAsync();

                // Keep reading through every row 
                while (await reader.ReadAsync())
                {
                    // create a new orderItem var
                    var orderItem = new OrderItemEntity
                    {
                        // assign the info from the columns to each of the properties of the orderItem
                        ItemID = reader.GetInt32(reader.GetOrdinal("ItemID")),
                        OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                        ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        ItemPrice = reader.GetDecimal(reader.GetOrdinal("ItemPrice")),
                        DiscountAmount = reader.GetDecimal(reader.GetOrdinal("DiscountAmount")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                    };
                    // add this orderItem to the list
                    orderItems.Add(orderItem);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving order-item list");
            }

            // Return the list of orderItems
            // if there is an error, it will return a semi-complete list (everything up to the error)
            return orderItems;
        }
    }
}
