using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGuitarShop.Common.Interfaces
{
    public interface IRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity?> FindByIDAsync(int id);

        Task<int> InsertAsync(TEntity entity);

        Task<int> UpdateAsync(int id, TEntity entity);

        Task<int> DeleteAsync(int id);
    }
}
