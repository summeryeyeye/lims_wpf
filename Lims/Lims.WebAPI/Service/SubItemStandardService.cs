using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service.Interface;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service
{
    public class SubItemStandardService : BaseService<SubItemStandardModel>, ISubItemStandardService
    {
        private readonly ISubItemStandardRepository iSubItemStandardRepository;

        public SubItemStandardService(ISubItemStandardRepository iSubItemStandardRepository,IMapper mapper)
        {
            base._iBaseRepository = iSubItemStandardRepository;
            this.iSubItemStandardRepository = iSubItemStandardRepository;
            base.mapper = mapper;
        }

        #region 查询函数


        public override async Task<ApiResponse> SearchAsync(dynamic primaryKey)
        {
            try
            {
                var data = await iSubItemStandardRepository.SearchAsync(primaryKey);
                var dataDto = mapper.Map<SubItemStandardDto>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<SubItemStandardModel, bool>> func)
        {
            try
            {
                var data = await iSubItemStandardRepository.QueryFirstOrDefaultAsync(func);
                var dataDto = mapper.Map<SubItemStandardDto>(data);
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
                var data = await iSubItemStandardRepository.QueryAsync();
                var dataDto = mapper.Map<List<SubItemStandardDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<SubItemStandardModel, bool>> func)
        {
            try
            {
                var data = await iSubItemStandardRepository.QueryAsync(func);
                var dataDto = mapper.Map<List<SubItemStandardDto>>(data);
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
                var data = await iSubItemStandardRepository.QueryAsync(page, size, total);
                var dataDto = mapper.Map<List<SubItemStandardDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<SubItemStandardModel, bool>> func, int page, int size, global::SqlSugar.RefAsync<int> total)
        {
            try
            {
                var data = await iSubItemStandardRepository.QueryAsync(func, page, size, total);
                var dataDto = mapper.Map<List<SubItemStandardDto>>(data);
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