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
    public class AdministratorRepo(
        ILogger<AdministratorRepo> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<AdministratorEntity>
    {
        public async Task<int> DeleteAsync(int id)
        {
            // create the query we will run
            const string query = @"DELETE FROM Administrators WHERE AdminID = @AdminID";

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand(query, connection);

                // set the variable with the id sent into this function
                command.Parameters.AddWithValue("@AdminID", id);

                // runs the query
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error Deleting Admins");
                return 0;
            }
        }

        public async Task<AdministratorEntity?> FindByIDAsync(int id)
        {
            // Create a admin var
            AdministratorEntity admin = null;

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand("SELECT * FROM Administrators WHERE AdminID = @AdminID", connection);

                // set the variable @AdminID with the id sent into this function
                command.Parameters.AddWithValue("@AdminID", id);

                // creates an SQL Data reader that will read data from our sql tables
                await using var reader = await command.ExecuteReaderAsync();

                // if the AdminID isn't null
                if (await reader.ReadAsync())
                {
                    admin = new AdministratorEntity
                    {
                        // assign the info from the columns to each of the properties of the admin
                        AdminID = reader.GetInt32(reader.GetOrdinal("AdminID")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error finding admin {id} by ID");
            }

            // return the admin, or null if an error occured
            return admin;
        }

        public async Task<IEnumerable<AdministratorEntity>> GetAllAsync()
        {
            // Create a list of admins
            var admins = new List<AdministratorEntity>();

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand("SELECT * FROM Administrators", connection);

                // creates an SQL Data reader that will read data from our sql tables
                await using var reader = await command.ExecuteReaderAsync();

                // Keep reading through every row 
                while (await reader.ReadAsync())
                {
                    // create a new admin var
                    var admin = new AdministratorEntity
                    {
                        // assign the info from the columns to each of the properties of the admin
                        AdminID = reader.GetInt32(reader.GetOrdinal("AdminID")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName"))
                    };
                    // add this admin to the list
                    admins.Add(admin);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving admin list");
            }

            // Return the list of admins
            // if there is an error, it will return a semi-complete list (everything up to the error)
            return admins;
        }

        public async Task<int> InsertAsync(AdministratorEntity entity)
        {
            // Create an insert query with variables
            const string query = @"
                INSERT INTO Administrators (EmailAddress, Password, FirstName, LastName) 
                VALUES (@EmailAddress, @Password, @FirstName, @LastName) ;";

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

                // run the query
                return await command.ExecuteNonQueryAsync();

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new admin");
                return 0;
            }
        }

        public async Task<int> UpdateAsync(int id, AdministratorEntity DTO)
        {
            // Create an update query with variables
            const string query = @"
                UPDATE Administrators
                SET EmailAddress = @EmailAddress, Password = @Password
                WHERE AdminID = @AdminID";

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand(query, connection);

                // set the variables with the entity sent into this function
                command.Parameters.AddWithValue("@AdminID", id);
                command.Parameters.AddWithValue("@EmailAddress", DTO.EmailAddress);
                command.Parameters.AddWithValue("@Password", DTO.Password);

                // run the query
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error updating admin");
                return 0;
            }
        }
    }
}
