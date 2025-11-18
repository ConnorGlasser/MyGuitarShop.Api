using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyGuitarShop.Data.EFCore.Context;

namespace MyGuitarShop.Data.EFCore.Factory
{
    public class MyGuitarShopContextFactory : IDesignTimeDbContextFactory<MyGuitarShopContext>
    {
        public MyGuitarShopContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // create a connection to the Migrations login for the database
            var connectionString = configuration.GetConnectionString("MyGuitarShopMigrations");

            // creates a builder for the myGuitarShopContext
            var optionsBuilder = new DbContextOptionsBuilder<MyGuitarShopContext>();

            // Connect the builder to the SQL Server using the login
            optionsBuilder.UseSqlServer(connectionString);

            // return the options as a MyGuitarShopContext
            return new MyGuitarShopContext(optionsBuilder.Options);
        }
                
    }
}
