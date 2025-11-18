using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Common.Interfaces;

namespace MyGuitarShop.API.Controllers.AdoControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(
        ILogger<CustomersController> logger,
        IRepository<CustomerDTO> repo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCustomerAsync()
        {
            try
            {
                var customers = await repo.GetAllAsync();

                return Ok(customers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving customers");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerByIDAsync(int id)
        {
            try
            {
                var customer = await repo.FindByIDAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving customer with ID {CustomerID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync(CustomerDTO newCustomer)
        {
            try
            {
                var numCustomersCreated = await repo.InsertAsync(newCustomer);

                return Ok($"{numCustomersCreated} new customers created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating customer");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, CustomerDTO updatedCustomer)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Customer with id {id} not found");

                var numCustomersUpdated = await repo.UpdateAsync(id, updatedCustomer);

                return Ok($"{numCustomersUpdated} new customers updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating customer with id {CustomerID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerAsync(int id)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Customer with id {id} not found");

                var numCustomersDeleted = await repo.DeleteAsync(id);

                return Ok($"{numCustomersDeleted} customers deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting customer with id {CustomerID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
