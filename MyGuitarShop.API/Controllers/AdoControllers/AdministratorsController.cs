using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Common.Interfaces;

namespace MyGuitarShop.API.Controllers.AdoControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorsController(
        ILogger<AdministratorsController> logger,
        IRepository<AdministratorDTO> repo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAdminsAsync()
        {
            try
            {
                var admins = await repo.GetAllAsync();

                return Ok(admins);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving admins");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminByIDAsync(int id)
        {
            try
            {
                var admin = await repo.FindByIDAsync(id);
                if (admin == null)
                {
                    return NotFound();
                }
                return Ok(admin);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving admin with ID {AdminID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdminAsync(AdministratorDTO newAdmin)
        {
            try
            {
                var numAdminsCreated = await repo.InsertAsync(newAdmin);

                return Ok($"{numAdminsCreated} new admins created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating admin");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, AdministratorDTO updatedAdmin)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Product with id {id} not found");

                var numAdminsUpdated = await repo.UpdateAsync(id, updatedAdmin);

                return Ok($"{numAdminsUpdated} new admins updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating admin with id {AdminID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminAsync(int id)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Admin with id {id} not found");

                var numAdminsDeleted = await repo.DeleteAsync(id);

                return Ok($"{numAdminsDeleted} admins deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting admin with id {AdminID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
