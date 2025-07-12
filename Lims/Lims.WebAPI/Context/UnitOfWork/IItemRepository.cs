using Lims.WebAPI.Models;
using System.Linq.Expressions;

namespace Lims.WebAPI.Context.UnitOfWork
{
    public interface IItemRepository : IBaseRepository<ItemModel>
    {
        Task<List<ItemModel>> RelativeQueryAsync(Expression<Func<ItemModel, bool>> func);
        Task<List<ItemModel>> QuerySampleOfItemAsync(Expression<Func<ItemModel, bool>> func);
        Task<List<ItemModel>> RelativeQueryAsync(Expression<Func<ItemModel, bool>> func, int pageNumber, int pageSize);
    }
}