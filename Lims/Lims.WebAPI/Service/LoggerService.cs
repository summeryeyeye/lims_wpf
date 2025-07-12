using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service.Interface;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service
{
    public class LoggerService : BaseService<LoggerModel>, ILoggerService
    {
        private readonly ILoggerRepository iLoggerRepository;

        public LoggerService(ILoggerRepository iLoggerRepository,IMapper mapper)
        {
            base._iBaseRepository = iLoggerRepository;
            base.mapper = mapper;
            this.iLoggerRepository = iLoggerRepository;
        }

        #region 查询函数


        public override async Task<ApiResponse> SearchAsync(dynamic primaryKey)
        {
            try
            {
                var data = await iLoggerRepository.SearchAsync(primaryKey);
                var dataDto = mapper.Map<LoggerDto>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<LoggerModel, bool>> func)
        {
            try
            {
                var data = await iLoggerRepository.QueryFirstOrDefaultAsync(func);
                var dataDto = mapper.Map<LoggerDto>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync()
        {
            try
            {
                var data = await iLoggerRepository.QueryAsync();
                var dataDto = mapper.Map<List<LoggerDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<LoggerModel, bool>> func)
        {
            try
            {
                var data = await iLoggerRepository.QueryAsync(func);
                var dataDto = mapper.Map<List<LoggerDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(int page, int size, global::SqlSugar.RefAsync<int> total)
        {
            try
            {
                var data = await iLoggerRepository.QueryAsync(page, size, total);

                var dataDto = mapper.Map<List<LoggerDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<LoggerModel, bool>> func, int page, int size, global::SqlSugar.RefAsync<int> total)
        {
            try
            {
                var data = await iLoggerRepository.QueryAsync(func, page, size, total);
                var dataDto = mapper.Map<List<LoggerDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }
        #endregion
    }
}