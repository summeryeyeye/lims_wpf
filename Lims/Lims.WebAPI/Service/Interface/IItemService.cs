using Lims.WebAPI.Models;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service.Interface
{
    public interface IItemService : IBaseService<ItemModel>
    {
        Task<ApiResponse> RelativeQueryAsync(Expression<Func<ItemModel, bool>> func);
        /// <summary>
        /// 通过项目获取样品
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<ApiResponse> QuerySampleOfItemAsync(Expression<Func<ItemModel, bool>> func);
        Task<ApiResponse> RelativeQueryAsync(Expression<Func<ItemModel, bool>> func, int pageNumber, int pageSize);
        //Task<ApiResponse> QueryWithFatherAsync(Expression<Func<ItemModel, bool>> func);
    }
}