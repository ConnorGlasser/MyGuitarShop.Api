using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGuitarShop.Data.Ado.Factories;
using MyGuitarShop.Data.EFCore.Context;


namespace MyGuitarShop.API.Controllers.AdoControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController(
        ILogger<HealthController> logger, 
        SqlConnectionFactory sqlConnectionFactory,
        MyGuitarShopContext dbContext)
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
    }
}
