using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Common.Interfaces;

namespace MyGuitarShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController (
        ILogger<OrderController> logger,
        IRepository<OrderDTO> repo) 
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync()
        {
            try
            {
                var orders = await repo.GetAllAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving orders");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByID(int id)
        {
            try
            {
                var order = await repo.FindByIDAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving order with ID {OrderID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync(OrderDTO newOrder)
        {
            try
            {
                var numOrdersCreated = await repo.InsertAsync(newOrder);

                return Ok($"{numOrdersCreated} new orders created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating order");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, OrderDTO updatedOrder)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Order with id {id} not found");

                var numOrdersUpdated = await repo.UpdateAsync(id, updatedOrder);

                return Ok($"{numOrdersUpdated} new orders updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating order with id {OrderID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderAsync(int id)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Order with id {id} not found");

                var numOrdersDeleted = await repo.DeleteAsync(id);

                return Ok($"{numOrdersDeleted} orders deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting order with id {OrderID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
