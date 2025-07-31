namespace Lims.WPF.Services.Interface
{
    public interface IBaseService<TEntity> where TEntity : class, new()
    {
        Task<ApiResponse<bool>> CreateAsync(TEntity entity);
        Task<ApiResponse<bool>> CreateRangeAsync(IEnumerable<TEntity> entities);
        Task<ApiResponse<bool>> DeleteAsync(dynamic primmaryKey);
        Task<ApiResponse<bool>> UpdateAsync(TEntity entity);
        Task<ApiResponse<bool>> UpdateRangeAsync(IEnumerable<TEntity> entities);



        Task<ApiResponse<TEntity>> GetSingleAsync(dynamic primmaryKey);
        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<List<TEntity>>> GetAllAsync();
    }
}