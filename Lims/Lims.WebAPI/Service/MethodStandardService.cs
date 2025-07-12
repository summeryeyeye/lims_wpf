using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service.Interface;
using System;
using System.Drawing;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service
{
    public class MethodStandardService : BaseService<MethodStandardModel>, IMethodStandardService
    {
        private readonly IMethodStandardRepository iMethodStandardRepository;

        public MethodStandardService(IMethodStandardRepository iMethodStandardRepository, IMapper mapper)
        {
            base._iBaseRepository = iMethodStandardRepository;
            this.iMethodStandardRepository = iMethodStandardRepository;
            base.mapper = mapper;
        }

        public override async Task<ApiResponse> SearchAsync(dynamic primaryKey)
        {
            try
            {
                var data = await iMethodStandardRepository.SearchAsync(System.Convert.ToInt32(primaryKey));
                var dataDto = mapper.Map<MethodStandardDto>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }
        #region 查询函数
        public override async Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<MethodStandardModel, bool>> func)
        {
            try
            {
                var data = await iMethodStandardRepository.QueryFirstOrDefaultAsync(func);
                var dataDto = mapper.Map<MethodStandardDto>(data);
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
                var data = await iMethodStandardRepository.QueryAsync();
                var dataDto = mapper.Map<List<MethodStandardDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<MethodStandardModel, bool>> func)
        {
            try
            {
                var data = await iMethodStandardRepository.QueryAsync(func);
                var dataDto = mapper.Map<List<MethodStandardDto>>(data);
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
                var data = await iMethodStandardRepository.QueryAsync(page, size, total);
                var dataDto = mapper.Map<List<MethodStandardDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<MethodStandardModel, bool>> func, int page, int size, global::SqlSugar.RefAsync<int> total)
        {
            try
            {
                var data = await iMethodStandardRepository.QueryAsync(func, page, size, total);
                var dataDto = mapper.Map<List<MethodStandardDto>>(data);
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
