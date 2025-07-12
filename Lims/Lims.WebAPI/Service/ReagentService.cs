using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service.Interface;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service
{
    public class ReagentService : BaseService<ReagentModel>, IReagentService
    {
        private readonly IReagentRepository iReagentRepository;

        public ReagentService(IReagentRepository iReagentRepository, IMapper mapper)
        {
            base._iBaseRepository = iReagentRepository;
            base.mapper = mapper;
            this.iReagentRepository = iReagentRepository;
        }

        #region 查询函数
        public override async Task<ApiResponse> SearchAsync(dynamic primaryKey)
        {
            try
            {
                var data = await iReagentRepository.SearchAsync(primaryKey);
                var dataDto = mapper.Map<ReagentDto>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<ReagentModel, bool>> func)
        {
            try
            {
                var data = await iReagentRepository.QueryFirstOrDefaultAsync(func);
                var dataDto = mapper.Map<ReagentDto>(data);
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
                var data = await iReagentRepository.QueryAsync();
                var dataDto = mapper.Map<List<ReagentDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<ReagentModel, bool>> func)
        {
            try
            {
                var data = await iReagentRepository.QueryAsync(func);
                var dataDto = mapper.Map<List<ReagentDto>>(data);
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
                var data = await iReagentRepository.QueryAsync(page, size, total);

                var dataDto = mapper.Map<List<ReagentDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<ReagentModel, bool>> func, int page, int size, global::SqlSugar.RefAsync<int> total)
        {
            try
            {
                var data = await iReagentRepository.QueryAsync(func, page, size, total);
                var dataDto = mapper.Map<List<ReagentDto>>(data);
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
