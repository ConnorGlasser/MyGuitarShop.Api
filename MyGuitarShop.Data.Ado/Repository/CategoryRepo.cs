using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Factories;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class CategoryRepo(
        ILogger<CategoryRepo> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<CategoryEntity>
    {
        public async Task<int> DeleteAsync(int id)
        {
            // create the query we will run
            const string query = @"DELETE FROM Categories WHERE CategoryID = @CategoryID";

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand(query, connection);

                // set the variable with the id sent into this function
                command.Parameters.AddWithValue("@CategoryID", id);

                // runs the query
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error Deleting Categories");
                return 0;
            }
        }

        public async Task<CategoryEntity?> FindByIDAsync(int id)
        {
            // Create a category var
            CategoryEntity category = null;

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand("SELECT * FROM Categories WHERE CategoryID = @CategoryID", connection);

                // set the variable @CategoryID with the id sent into this function
                command.Parameters.AddWithValue("@CategoryID", id);

                // creates an SQL Data reader that will read data from our sql tables
                await using var reader = await command.ExecuteReaderAsync();

                // if the CategoryID isn't null
                if (await reader.ReadAsync())
                {
                    category = new CategoryEntity
                    {
                        // assign the info from the columns to each of the properties of the category
                        CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error finding category {id} by ID");
            }

            // return the category, or null if an error occured
            return category;
        }

        public async Task<IEnumerable<CategoryEntity>> GetAllAsync()
        {
            // Create a list of categories
            var categories = new List<CategoryEntity>();

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand("SELECT * FROM Categories", connection);

                // creates an SQL Data reader that will read data from our sql tables
                await using var reader = await command.ExecuteReaderAsync();

                // Keep reading through every row 
                while (await reader.ReadAsync())
                {
                    // create a new category var
                    var category = new CategoryEntity
                    {
                        // assign the info from the columns to each of the properties of the category
                        CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                    };
                    // add this category to the list
                    categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving categories list");
            }

            // Return the list of categories
            // if there is an error, it will return a semi-complete list (everything up to the error)
            return categories;
        }

        public async Task<int> InsertAsync(CategoryEntity entity)
        {
            // Create an insert query with variables
            const string query = @"
                INSERT INTO Categories (CategoryName)
                VALUES (@CategoryName);";

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand(query, connection);

                // set the variable with the entity sent into this function
                command.Parameters.AddWithValue("@CategoryName", entity.CategoryName);

                // run the query
                return await command.ExecuteNonQueryAsync();

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new category");
                return 0;
            }
        }

        public async Task<int> UpdateAsync(int id, CategoryEntity DTO)
        {
            // Create an update query with variables
            const string query = @"
                UPDATE Products
                SET CategoryName = @CategoryName
                WHERE CategoryID = @CategoryID";

            try
            {
                // Gets a connection to the sql database
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();

                // creates an SQL command that uses said connection
                await using var command = new SqlCommand(query, connection);

                // set the variables with the entity sent into this function
                command.Parameters.AddWithValue("@CategoryID", id);
                command.Parameters.AddWithValue("@CategoryName", DTO.CategoryName);

                // run the query
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error updating category");
                return 0;
            }
        }
    }
}
