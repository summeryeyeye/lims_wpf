using Lims.WebAPI.Models;
using SqlSugar;
using System.Linq.Expressions;

namespace Lims.WebAPI.Context.UnitOfWork
{
    public interface ISampleRepository : IBaseRepository<SampleModel>
    {
        public Task<List<SampleModel>> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func1, Expression<Func<SampleModel, List<ItemModel>>> func2, int pageNumber, int pageSize, RefAsync<int> total, string orderFileds);

        public Task<List<SampleModel>> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func1, Expression<Func<SampleModel, List<ItemModel>>> func2, string orderFileds);

        public Task<List<SampleModel>> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func, string orderFileds);

        public Task<List<SampleModel>> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func, int pageNumber, int pageSize);
    }
}