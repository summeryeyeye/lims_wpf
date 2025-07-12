using SqlSugar;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service.Interface
{
    public interface IBaseService<TEntity> where TEntity : class, new()
    {
        Task<ApiResponse> CreateAsync(TEntity entity);

        Task<ApiResponse> DeleteAsync(string primmaryKey);

        Task<ApiResponse> EditAsync(TEntity entity);

        Task<ApiResponse> EditRangeAsync(List<TEntity> updateObjs);

        Task<ApiResponse> SearchAsync(dynamic primmaryKey);

        Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<TEntity, bool>> func);

        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse> QueryAsync();

        Task<ApiResponse> AnyAsync(Expression<Func<TEntity, bool>> func);

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<ApiResponse> QueryAsync(Expression<Func<TEntity, bool>> func);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<ApiResponse> QueryAsync(int page, int size, RefAsync<int> total);

        /// <summary>
        /// 条件查询分页
        /// </summary>
        /// <param name="func"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<ApiResponse> QueryAsync(Expression<Func<TEntity, bool>> func, int page, int size, RefAsync<int> total);
    }
}