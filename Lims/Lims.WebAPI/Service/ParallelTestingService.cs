using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Context.Repository;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service.Interface;
using System;
using System.Drawing;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service
{
    public class ParallelTestingService : BaseService<ParallelTestingModel>, IParallelTestingService
    {
        private readonly IParallelTestingRepository iParallelTestingRepository;

        public ParallelTestingService(IParallelTestingRepository iParallelTestingRepository, IMapper mapper)
        {
            base._iBaseRepository = iParallelTestingRepository;
            base.mapper = mapper;
            this.iParallelTestingRepository = iParallelTestingRepository;
        }
        public async Task<ApiResponse> GetParallelTestingsByParentIdAsync(Expression<Func<ParallelTestingModel, bool>> func)
        {
            try
            {
                var data = await iParallelTestingRepository.QueryAsync(func);
                var dataDto = mapper.Map<List<ParallelTestingDto>>(data);
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
                var data = await iParallelTestingRepository.SearchAsync(primaryKey);
                var dataDto = mapper.Map<ParallelTestingDto>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<ParallelTestingModel, bool>> func)
        {
            try
            {
                var data = await iParallelTestingRepository.QueryFirstOrDefaultAsync(func);
                var dataDto = mapper.Map<ParallelTestingDto>(data);
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
                var data = await iParallelTestingRepository.QueryAsync();
                var dataDto = mapper.Map<List<ParallelTestingDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<ParallelTestingModel, bool>> func)
        {
            try
            {
                var data = await iParallelTestingRepository.QueryAsync(func);
                var dataDto = mapper.Map<List<ParallelTestingDto>>(data);
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
                var data = await iParallelTestingRepository.QueryAsync(page, size, total);

                var dataDto = mapper.Map<List<ParallelTestingDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<ParallelTestingModel, bool>> func, int page, int size, global::SqlSugar.RefAsync<int> total)
        {
            try
            {
                var data = await iParallelTestingRepository.QueryAsync(func, page, size, total);
                var dataDto = mapper.Map<List<ParallelTestingDto>>(data);
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
