using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
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
    public class AddressRepo(
        ILogger<AddressRepo> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<AddressEntity>
    {
        public async Task<IEnumerable<AddressEntity>> GetAllAsync()
        {
            // Create a list of addresses
            var addresses = new List<AddressEntity>();

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand("SELECT * FROM Addresses", connection);

                // creates an SQL Data reader that will read data from our sql tables
                await using var reader = await command.ExecuteReaderAsync();

                // Keep reading through every row 
                while (await reader.ReadAsync())
                {
                    // create a new address var
                    var address = new AddressEntity
                    {
                        // assign the info from the columns to each of the properties of the address
                        AddressID = reader.GetInt32(reader.GetOrdinal("AddressID")),
                        CustomerID = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        Line1 = reader.GetString(reader.GetOrdinal("Line1")),
                        Line2 = reader.GetString(reader.GetOrdinal("Line2")),
                        City = reader.GetString(reader.GetOrdinal("City")),
                        State = reader.GetString(reader.GetOrdinal("State")),
                        ZipCode = reader.GetString(reader.GetOrdinal("ZipCode")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        Disabled = reader.GetInt32(reader.GetOrdinal("Disabled"))
                    };
                    // add this address to the list
                    addresses.Add(address);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving address list");
            }

            // Return the list of addresses
            // if there is an error, it will return a semi-complete list (everything up to the error)
            return addresses;
        }
    }
}
