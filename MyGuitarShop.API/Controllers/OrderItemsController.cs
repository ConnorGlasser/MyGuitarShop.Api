using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Common.Interfaces;

namespace MyGuitarShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController(
        ILogger<OrderItemsController> logger,
        IRepository<OrderItemDTO> repo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetOrderItemsAsync()
        {
            try
            {
                var orderItems = await repo.GetAllAsync();

                return Ok(orderItems);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving order items");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderItemByIDAsync(int id)
        {
            try
            {
                var orderItem = await repo.FindByIDAsync(id);
                if (orderItem == null)
                {
                    return NotFound();
                }
                return Ok(orderItem);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving order item with ID {ItemID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync(OrderItemDTO newOrderItem)
        {
            try
            {
                var numOrderItemsCreated = await repo.InsertAsync(newOrderItem);

                return Ok($"{numOrderItemsCreated} new products created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating order item");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, OrderItemDTO updatedOrderItem)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Order item with id {id} not found");

                var numOrderItemsUpdated = await repo.UpdateAsync(id, updatedOrderItem);

                return Ok($"{numOrderItemsUpdated} new products updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating order item with id {ItemID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItemAsync(int id)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Product with id {id} not found");

                var numOrderItemsDeletesd = await repo.DeleteAsync(id);

                return Ok($"{numOrderItemsDeletesd} order items deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting order item with id {ItemID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
