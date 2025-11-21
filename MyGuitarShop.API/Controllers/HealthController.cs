using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MyGuitarShop.Data.Ado.Factories;
using MyGuitarShop.Data.EFCore.Context;


namespace MyGuitarShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController(
        ILogger<HealthController> logger, 
        SqlConnectionFactory sqlConnectionFactory,
        MyGuitarShopContext dbContext,
        IMongoClient mongoClient)
        : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok("Healthy");
            }
            catch(Exception ex)
            {
                logger.LogWarning("Health Check Failed Unreasonably");
                return StatusCode(503, "Unhealthy");
            }
        }

        [HttpGet("db/ado")]
        public IActionResult GetDBHealth()
        {
            try
            {
                using var connection = sqlConnectionFactory.OpenSqlConnection();
                return Ok(new {Message="Connection Successful!", connection.Database});
            }
            catch(Exception ex)
            {
                logger.LogCritical("Database health check failed");
                return StatusCode(503, "Unhealthy");
            }
        }

        [HttpGet("db/efcore")]
        public async Task<IActionResult> GetDbContextHealthAsync()
        {
            try
            {
                if (!await dbContext.Database.CanConnectAsync())
                    throw new Exception("Cannot to the database via EF Core DbContext.");

                return Ok(new { Message = "EFCore DbContext Connection Successful!", dbContext.Database.GetDbConnection().Database });
            }
            catch (Exception)
            {
                logger.LogCritical("EfCore DbContext health check failed");
                return StatusCode(503, "Database Unhealthy via EFCore DbContext");
            }
        }

        [HttpGet("db/mongo")]
        public async Task<IActionResult> GetMongoDbHealthAsync()
        {
            try
            {
                var response = await mongoClient.ListDatabaseNamesAsync();
                var databaseNames = await response.ToListAsync() ?? [];
                if (databaseNames.Count == 0)
                    throw new Exception("Cannot to Mongo Database.");

                return Ok(new { Message = "Mongo database connection successful!", databaseNames });
            }
            catch (Exception)
            {
                logger.LogCritical("Mongo database health check failed");
                return StatusCode(503, "Database Unhealthy via Mongo");
            }
        }
    }
}
