using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.API.Mappers;
using MyGuitarShop.Common.Interfaces;

namespace MyGuitarShop.API.Abstract
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<TDTO, TEntity> (
        ILogger<BaseController<TDTO, TEntity>> logger,
        IRepository<TEntity> repo) : ControllerBase
        where TEntity : class, new()
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var entities = await repo.GetAllAsync();

                return entities.Any() ? Ok(entities) : NotFound();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving items");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIDAsync (int id)
        {
            try
            {
                var entity = await repo.FindByIDAsync(id);

                if (entity == null)
                    return NotFound($"Entity with id {id} not found");

                return Ok(entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching entity");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(TDTO dto)
        {
            try
            {
                var entity = AutoReflectionMapper.Map<TDTO, TEntity>(dto);

                if (entity == null)
                    throw new Exception("Mapping resulted in null entity");

                var entitiesCreated = await repo.InsertAsync(entity);

                return Ok($"{entitiesCreated} new entities created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating entity");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, TDTO dto)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Entity with id {id} not found");

                var entity = AutoReflectionMapper.Map<TDTO, TEntity>(dto)
                    ?? throw new Exception("Mapping resulted in null entity");

                var entitiesUpdated = await repo.UpdateAsync(id, entity);

                return Ok($"{entitiesUpdated} entities updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating entity with id {EntityID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                if (await repo.FindByIDAsync(id) == null)
                    return NotFound($"Entity with id {id} not found");

                var entitiesDeleted = await repo.DeleteAsync(id);

                return Ok($"{entitiesDeleted} entities deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting entity with id {EntityID}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

    }
}
