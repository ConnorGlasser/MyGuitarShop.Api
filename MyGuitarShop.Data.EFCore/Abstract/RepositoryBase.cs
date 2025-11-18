using Microsoft.EntityFrameworkCore;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.EFCore.Context;

namespace MyGuitarShop.Data.EFCore.Abstract
{
    public abstract class RepositoryBase<TEntity>(
        MyGuitarShopContext dbContext
        ) : IRepository<TEntity> 
        where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

        public async Task<int> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id); // find the entity
            if (entity == null) // if it doesn't exist
                return 0;       // do nothing

            _dbSet.Remove(entity); // otherwise remove it 

            return await dbContext.SaveChangesAsync();  // only save the changes if the delete worked
        }

        public async Task<TEntity?> FindByIDAsync(int id) =>
            await _dbSet.FindAsync(id);

        public async Task<IEnumerable<TEntity>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public async Task<int> InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity); // add the entity to the database

            return await dbContext.SaveChangesAsync(); // only save the changes if the delete worked
        }

        public async Task<int> UpdateAsync(int id, TEntity entity)
        {
            var existingEntity = await _dbSet.FindAsync(id); // find the entity

            if (existingEntity == null) // if it doesn't exist
                return 0;               // don't do anything

            _dbSet.Entry(existingEntity).CurrentValues.SetValues(entity); // set the values equal to the new entity

            return await dbContext.SaveChangesAsync();  // only save the changes if the delete worked
        }
    }
}
