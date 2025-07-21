using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Context.Repository;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service.Interface;
using SqlSugar;
using System;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service
{
    public class ParalleltestingService : BaseService<ParalleltestingModel>, IParalleltestingService
    {
        private readonly IParalleltestingRepository _iParalleltestingRepository;

        public ParalleltestingService(IParalleltestingRepository iParalleltestingRepository, IMapper mapper)
        {
            base._iBaseRepository = iParalleltestingRepository;
            this._iParalleltestingRepository = iParalleltestingRepository;
            base.mapper = mapper;

        }
        public async override Task<ApiResponse> QueryAsync()
        {
            try
            {
                var data = await _iParalleltestingRepository.QueryAsync();
                var dataDto = mapper.Map<List<ParalleltestingModel>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async override Task<ApiResponse> QueryAsync(Expression<Func<ParalleltestingModel, bool>> func)
        {
            try
            {

                var data = await _iParalleltestingRepository.QueryAsync(func);
                var dataDto = mapper.Map<List<ParalleltestingModel>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async override Task<ApiResponse> QueryAsync(int page, int size, RefAsync<int> total)
        {
            try
            {
                var data = await _iParalleltestingRepository.QueryAsync(page, size, total);
                var dataDto = mapper.Map<List<ParalleltestingModel>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async override Task<ApiResponse> QueryAsync(Expression<Func<ParalleltestingModel, bool>> func, int page, int size, RefAsync<int> total)
        {
            try
            {
                var data = await _iParalleltestingRepository.QueryAsync(func, page, size, total);
                var dataDto = mapper.Map<List<ParalleltestingModel>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async override Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<ParalleltestingModel, bool>> func)
        {
            try
            {
                var data = await _iParalleltestingRepository.QueryFirstOrDefaultAsync(func);
                var dataDto = mapper.Map<ParalleltestingModel>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async override Task<ApiResponse> SearchAsync(dynamic primaryKey)
        {
            try
            {
                var data = await _iParalleltestingRepository.SearchAsync(primaryKey);
                var dataDto = mapper.Map<ParalleltestingModel  >(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }
    }
}
