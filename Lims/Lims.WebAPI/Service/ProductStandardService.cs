using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service.Interface;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service
{
    public class ProductStandardService : BaseService<ProductStandardModel>, IProductStandardService
    {
        private readonly IProductStandardRepository iProductStandardRepository;

        public ProductStandardService(IProductStandardRepository iProductStandardRepository, IMapper mapper)
        {
            base._iBaseRepository = iProductStandardRepository;
            this.iProductStandardRepository = iProductStandardRepository;
            base.mapper = mapper;
        }

        #region 查询函数


        public override async Task<ApiResponse> SearchAsync(dynamic primaryKey)
        {
            try
            {
                var data = await iProductStandardRepository.SearchAsync(System.Convert.ToInt32(primaryKey));
                var dataDto = mapper.Map<ProductStandardDto>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<ProductStandardModel, bool>> func)
        {
            try
            {
                var data = await iProductStandardRepository.QueryFirstOrDefaultAsync(func);
                var dataDto = mapper.Map<ProductStandardDto>(data);
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
                var data = await iProductStandardRepository.QueryAsync();
                var dataDto = mapper.Map<List<ProductStandardDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<ProductStandardModel, bool>> func)
        {
            try
            {
                var data = await iProductStandardRepository.QueryAsync(func);
                var dataDto = mapper.Map<List<ProductStandardDto>>(data);
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
                var data = await iProductStandardRepository.QueryAsync(page, size, total);
                var dataDto = mapper.Map<List<ProductStandardDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<ProductStandardModel, bool>> func, int page, int size, global::SqlSugar.RefAsync<int> total)
        {
            try
            {
                var data = await iProductStandardRepository.QueryAsync(func, page, size, total);
                var dataDto = mapper.Map<List<ProductStandardDto>>(data);
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
