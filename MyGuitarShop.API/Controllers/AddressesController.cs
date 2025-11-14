using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Common.Interfaces;

namespace MyGuitarShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController(
        ILogger<AddressesController> logger,
        IRepository<AddressDTO> repo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAddressesAsync()
        {
            try
            {
                var addresses = await repo.GetAllAsync();

                return Ok(addresses);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving addresses");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressByIDAsync(int id)
        {
            try
            {
                var address = await repo.FindByIDAsync(id);
                if (address == null)
                {
                    return NotFound();
                }
                return Ok(address);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving address with ID {ProductID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddressAsync(AddressDTO newAddress)
        {
            try
            {
                var numAddressesCreated = await repo.InsertAsync(newAddress);

                return Ok($"{numAddressesCreated} new addresses created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating address");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddressAsync(int id, AddressDTO updatedAddress)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Product with id {id} not found");

                var numAddressesUpdated = await repo.UpdateAsync(id, updatedAddress);

                return Ok($"{numAddressesUpdated} new addresses updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating address with id {AddressID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddressAsync(int id)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Address with id {id} not found");

                var numAddressesDeleted = await repo.DeleteAsync(id);

                return Ok($"{numAddressesDeleted} addresses deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting address with id {AddressID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
