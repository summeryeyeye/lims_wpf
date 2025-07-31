using AutoMapper;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Service.Interface;
using SqlSugar;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service
{
    public abstract class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
    {
        protected IBaseRepository<TEntity> _iBaseRepository;
        protected IMapper mapper;
        public async Task<ApiResponse> CreateAsync(TEntity entity)
        {
            try
            {
                var data = await _iBaseRepository.CreateAsync(entity);

                return new ApiResponse(true, data);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }

        }
        public async Task<ApiResponse> CreateRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                var data = await _iBaseRepository.CreateRangeAsync(entities);

                return new ApiResponse(true, data);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }

        }
        public async Task<ApiResponse> DeleteAsync(string primmaryKey)
        {
            try
            {
                var data = await _iBaseRepository.DeleteAsync(primmaryKey);

                return new ApiResponse(true, data);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }
        public async Task<ApiResponse> EditAsync(TEntity entity)
        {
            try
            {
                var data = await _iBaseRepository.EditAsync(entity);

                return new ApiResponse(true, data);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> AnyAsync(Expression<Func<TEntity, bool>> func)
        {
            try
            {
                var data = await _iBaseRepository.AnyAsync(func);

                return new ApiResponse(true, data);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }
        public async Task<ApiResponse> EditRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                var data = await _iBaseRepository.EditRangeAsync(entities);

                return new ApiResponse(true, data);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public abstract Task<ApiResponse> SearchAsync(dynamic primaryKey);

        public abstract Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<TEntity, bool>> func);
        public abstract Task<ApiResponse> QueryAsync();

        public abstract Task<ApiResponse> QueryAsync(Expression<Func<TEntity, bool>> func);

        public abstract Task<ApiResponse> QueryAsync(int page, int size, RefAsync<int> total);

        public abstract Task<ApiResponse> QueryAsync(Expression<Func<TEntity, bool>> func, int page, int size, RefAsync<int> total);
    }
}