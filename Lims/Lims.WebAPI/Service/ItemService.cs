using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service.Interface;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service
{
    public class ItemService : BaseService<ItemModel>, IItemService
    {
        private readonly IItemRepository iItemRepository;
        

        public ItemService(IItemRepository iItemRepository, IMapper mapper)
        {
            base._iBaseRepository = iItemRepository;
            this.iItemRepository = iItemRepository;
            base.mapper = mapper;
        }

        public async Task<ApiResponse> RelativeQueryAsync(Expression<Func<ItemModel, bool>> func)
        {
            try
            {
                List<ItemModel> data = await iItemRepository.RelativeQueryAsync(func);

                List<ItemDto> dataDto = mapper.Map<List<ItemDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {

                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> RelativeQueryAsync(Expression<Func<ItemModel, bool>> func, int pageNumber, int pageSize)
        {
            try
            {
                var data = await iItemRepository.RelativeQueryAsync(func, pageNumber, pageSize);

                var dataDto = mapper.Map<List<ItemDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {

                return new ApiResponse(ex.Message);
            }
        }

        #region 查询函数


        public override async Task<ApiResponse> SearchAsync(dynamic primaryKey)
        {
            try
            {
                var data = await iItemRepository.SearchAsync(primaryKey);
                var dataDto = mapper.Map<ItemDto>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<ItemModel, bool>> func)
        {
            try
            {
                var data = await iItemRepository.QueryFirstOrDefaultAsync(func);
                var dataDto = mapper.Map<ItemDto>(data);
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
                var data = await iItemRepository.QueryAsync();
                var dataDto = mapper.Map<List<ItemDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<ItemModel, bool>> func)
        {
            try
            {
                
                var data = await iItemRepository.QueryAsync(func);
                var dataDto = mapper.Map<List<ItemDto>>(data);
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
                var data = await iItemRepository.QueryAsync(page, size, total);
                var dataDto = mapper.Map<List<ItemDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<ItemModel, bool>> func, int page, int size, global::SqlSugar.RefAsync<int> total)
        {
            try
            {
                var data = await iItemRepository.QueryAsync(func, page, size, total);
                var dataDto = mapper.Map<List<ItemDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> QuerySampleOfItemAsync(Expression<Func<ItemModel, bool>> func)
        {
            try
            {
                var data = await iItemRepository.QuerySampleOfItemAsync(func);
                var dataDto = mapper.Map<List<ItemDto>>(data);
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