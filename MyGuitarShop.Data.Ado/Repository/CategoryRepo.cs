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
                    // create a new product var
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
    }
}
