namespace Lims.WPF.Services.Interface
{
    public interface IBaseService<TEntity> where TEntity : class, new()
    {
        Task<ApiResponse<bool>> CreateAsync(TEntity entity);
        Task<ApiResponse<bool>> DeleteAsync(dynamic? primmaryKey);
        Task<ApiResponse<bool>> UpdateAsync(TEntity entity);
        Task<ApiResponse<bool>> UpdateRangeAsync(List<TEntity> updateObjs);



        Task<ApiResponse<TEntity>> GetSingleAsync(dynamic? primmaryKey);
        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<List<TEntity>>> GetAllAsync();
    }
}