using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Common.Interfaces;

namespace MyGuitarShop.API.Controllers.AdoControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(
        ILogger<CategoriesController> logger,
        IRepository<CategoryDTO> repo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            try
            {
                var categories = await repo.GetAllAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving categories");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIDAsync(int id)
        {
            try
            {
                var category = await repo.FindByIDAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving category with ID {CategoryID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync(CategoryDTO newCategory)
        {
            try
            {
                var numCategoriesCreated = await repo.InsertAsync(newCategory);

                return Ok($"{numCategoriesCreated} new categories created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating category");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync(int id, CategoryDTO updatedCategory)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Category with id {id} not found");

                var numCategoriesUpdated = await repo.UpdateAsync(id, updatedCategory);

                return Ok($"{numCategoriesUpdated} new categories updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating category with id {CategoryID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Category with id {id} not found");

                var numCategoriesDeleted = await repo.DeleteAsync(id);

                return Ok($"{numCategoriesDeleted} categories deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting category with id {CategoryID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
