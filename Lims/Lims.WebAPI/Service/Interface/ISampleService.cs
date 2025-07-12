using Lims.WebAPI.Models;
using SqlSugar;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service.Interface
{
    public interface ISampleService : IBaseService<SampleModel>
    {
        public Task<ApiResponse> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func1, Expression<Func<SampleModel, List<ItemModel>>> func2, int pageNumber, int pageSize, RefAsync<int> total, string orderFileds = "SampleCode");

        public Task<ApiResponse> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func1, Expression<Func<SampleModel, List<ItemModel>>> func2, string orderFileds = "SampleCode");

        public Task<ApiResponse> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func, string orderFileds = "SampleCode");

        public Task<ApiResponse> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func, int pageNumber, int pageSize);
    }
}