using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Data.Ado.Factories;

namespace MyGuitarShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController(
        ILogger<HealthController> logger, 
        SqlConnectionFactory sqlConnectionFactory)
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

        [HttpGet("db")]
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
    }
}
