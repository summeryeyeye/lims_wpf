using SqlSugar;
using System.Linq.Expressions;

namespace Lims.WebAPI.Context.UnitOfWork
{
    public interface IBaseRepository<TEntity> where TEntity : class, new()
    {
        Task<bool> CreateAsync(TEntity entity);

        Task<bool> DeleteAsync(string primmaryKey);
        //Task<bool> CreateRangeAsync(TEntity[] entitys);
        //Task<bool> DeleteRangeAsync(dynamic[] primmaryKeys);

        Task<bool> EditAsync(TEntity entity);

        Task<bool> EditRangeAsync(List<TEntity> updateObjs);

        Task<TEntity> SearchAsync(string primmaryKey);

        Task<TEntity> QueryFirstOrDefaultAsync(Expression<Func<TEntity, bool>> func);

        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> QueryAsync();

        /// <summary>
        ///
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> func);

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<List<TEntity>> QueryAsync(int page, int size, RefAsync<int> total);

        /// <summary>
        /// 条件查询分页
        /// </summary>
        /// <param name="func"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func, int page, int size, RefAsync<int> total);
    }
}