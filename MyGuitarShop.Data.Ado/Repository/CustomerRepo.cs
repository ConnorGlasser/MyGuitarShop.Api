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
    public class CustomerRepo(
        ILogger<CustomerRepo> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<CustomerEntity>
    {
        public async Task<int> DeleteAsync(int id)
        {
            // create the query we will run
            const string query = @"DELETE FROM Customers WHERE CustomerID = @CustomerID";

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand(query, connection);

                // set the variable with the id sent into this function
                command.Parameters.AddWithValue("@CustomerID", id);

                // runs the query
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error Deleting Customer");
                return 0;
            }
        }

        public async Task<CustomerEntity?> FindByIDAsync(int id)
        {
            // Create a customer var
            CustomerEntity customer = null;

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand("SELECT * FROM Customers WHERE CustomerID = @CustomerID", connection);

                // set the variable @CustomerID with the id sent into this function
                command.Parameters.AddWithValue("@CustomerID", id);

                // creates an SQL Data reader that will read data from our sql tables
                await using var reader = await command.ExecuteReaderAsync();

                // if the CustomerID isn't null
                if (await reader.ReadAsync())
                {
                    customer = new CustomerEntity
                    {
                        // assign the info from the columns to each of the properties of the customer
                        CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        ShippingAddressID = reader.IsDBNull(reader.GetOrdinal("ShippingAddressID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ShippingAddressID")),
                        BillingAddressID = reader.IsDBNull(reader.GetOrdinal("BillingAddressID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error finding customer {id} by ID");
            }

            // return the customer, or null if an error occured
            return customer;
        }

        public async Task<IEnumerable<CustomerEntity>> GetAllAsync()
        {
            // Create a list of categories
            var customers = new List<CustomerEntity>();

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
                    // create a new customer var
                    var customer = new CustomerEntity
                    {
                        // assign the info from the columns to each of the properties of the customer
                        CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        ShippingAddressID = reader.IsDBNull(reader.GetOrdinal("ShippingAddressID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ShippingAddressID")),
                        BillingAddressID = reader.IsDBNull(reader.GetOrdinal("BillingAddressID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
                    };
                    // add this customer to the list
                    customers.Add(customer);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving customer list");
            }

            // Return the list of customers
            // if there is an error, it will return a semi-complete list (everything up to the error)
            return customers;
        }

        public async Task<int> InsertAsync(CustomerEntity entity)
        {
            // Create an insert query with variables
            const string query = @"
                INSERT INTO Customers (EmailAddress, Password, FirstName, LastName, ShippingAddressID, BillingAddressID) 
                VALUES (@EmailAddress, @Password, @FirstName, @LastName, @ShippingAddressID, @BillingAddressID);";

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand(query, connection);

                // set the variable with the entity sent into this function
                command.Parameters.AddWithValue("@EmailAddress", entity.EmailAddress);
                command.Parameters.AddWithValue("@Password", entity.Password);
                command.Parameters.AddWithValue("@FirstName", entity.FirstName);
                command.Parameters.AddWithValue("@LastName", entity.LastName);
                command.Parameters.AddWithValue("@ShippingAddressID", entity.ShippingAddressID);
                command.Parameters.AddWithValue("@BillingAddressID", entity.BillingAddressID);

                // run the query
                return await command.ExecuteNonQueryAsync();

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new customer");
                return 0;
            }
        }

        public async Task<int> UpdateAsync(int id, CustomerEntity DTO)
        {
            // Create an update query with variables
            const string query = @"
                UPDATE Customers
                SET EmailAddress = @EmailAddress, Password = @Password, FirstName = @FirstName, LastName = @LastName,
                    ShippingAddressID = @ShippingAddressID, BillingAddressID = @BillingAddressID
                WHERE CustomerID = @CustomerID";

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand(query, connection);

                // set the variables with the entity sent into this function
                command.Parameters.AddWithValue("@CustomerID", id);
                command.Parameters.AddWithValue("@EmailAddress", DTO.EmailAddress);
                command.Parameters.AddWithValue("@Password", DTO.Password);
                command.Parameters.AddWithValue("@FirstName", DTO.FirstName);
                command.Parameters.AddWithValue("@LastName", DTO.LastName);
                command.Parameters.AddWithValue("@ShippingAddressID", DTO.ShippingAddressID);
                command.Parameters.AddWithValue("@BillingAddressID", DTO.BillingAddressID);

                // run the query
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error updating customer");
                throw;
            }
        }
    }
}
