using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service.Interface;
using SqlSugar;

using System.Linq.Expressions;

namespace Lims.WebAPI.Service

{
    public class SampleService : BaseService<SampleModel>, ISampleService
    {
        private readonly ISampleRepository iSampleRepository;

        public SampleService(ISampleRepository iSampleRepository, IMapper mapper)
        {
            base._iBaseRepository = iSampleRepository;
            this.iSampleRepository = iSampleRepository;
            base.mapper = mapper;
        }

        public async Task<ApiResponse> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func1, Expression<Func<SampleModel, List<ItemModel>>> func2, int pageNumber, int pageSize, RefAsync<int> total, string orderFileds)
        {
            try
            {
                var data = await iSampleRepository.QueryWithChildObjectsAsync(func1, func2, pageNumber, pageSize, total, orderFileds);
                var dataDto = mapper.Map<List<SampleDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func1, Expression<Func<SampleModel, List<ItemModel>>> func2, string orderFileds)
        {
            try
            {
                var data = await iSampleRepository.QueryWithChildObjectsAsync(func1, func2, orderFileds);
                var dataDto = mapper.Map<List<SampleDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func, string orderFileds)
        {
            try
            {
                var data = await iSampleRepository.QueryWithChildObjectsAsync(func, orderFileds);
                var dataDto = mapper.Map<List<SampleDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func, int pageNumber, int pageSize)
        {
            try
            {
                var data = await iSampleRepository.QueryWithChildObjectsAsync(func, pageNumber, pageSize);
                var dataDto = mapper.Map<List<SampleDto>>(data);
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
                var data = await iSampleRepository.SearchAsync(primaryKey);
                var dataDto = mapper.Map<SampleDto>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<SampleModel, bool>> func)
        {
            try
            {
                var data = await iSampleRepository.QueryFirstOrDefaultAsync(func);
                var dataDto = mapper.Map<SampleDto>(data);
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
                var data = await iSampleRepository.QueryAsync();
                var dataDto = mapper.Map<List<SampleDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<SampleModel, bool>> func)
        {
            try
            {
                var data = await iSampleRepository.QueryAsync(func);
                var dataDto = mapper.Map<List<SampleDto>>(data);
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
                var data = await iSampleRepository.QueryAsync(page, size, total);
                var dataDto = mapper.Map<List<SampleDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<SampleModel, bool>> func, int page, int size, global::SqlSugar.RefAsync<int> total)
        {
            try
            {
                var data = await iSampleRepository.QueryAsync(func, page, size, total);
                var dataDto = mapper.Map<List<SampleDto>>(data);
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