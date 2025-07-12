using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service.Interface;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service
{
    public class SubItemService : BaseService<SubItemModel>, ISubItemService
    {
        private readonly ISubItemRepository _iSubitemRepository;

        public SubItemService(ISubItemRepository iSubitemRepository ,IMapper mapper)
        {
            base._iBaseRepository = iSubitemRepository;
            this._iSubitemRepository = iSubitemRepository;
            base.mapper = mapper;
        }
        #region 查询函数


        public override async Task<ApiResponse> SearchAsync(dynamic primaryKey)
        {
            try
            {
                var data = await _iSubitemRepository.SearchAsync(primaryKey);
                var dataDto = mapper.Map<SubItemDto>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<SubItemModel, bool>> func)
        {
            try
            {
                var data = await _iSubitemRepository.QueryFirstOrDefaultAsync(func);
                var dataDto = mapper.Map<SubItemDto>(data);
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
                var data = await _iSubitemRepository.QueryAsync();
                var dataDto = mapper.Map<List<SubItemDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<SubItemModel, bool>> func)
        {
            try
            {
                var data = await _iSubitemRepository.QueryAsync(func);
                var dataDto = mapper.Map<List<SubItemDto>>(data);
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
                var data = await _iSubitemRepository.QueryAsync(page, size, total);
                var dataDto = mapper.Map<List<SubItemDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<SubItemModel, bool>> func, int page, int size, global::SqlSugar.RefAsync<int> total)
        {
            try
            {
                var data = await _iSubitemRepository.QueryAsync(func, page, size, total);
                var dataDto = mapper.Map<List<SubItemDto>>(data);
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