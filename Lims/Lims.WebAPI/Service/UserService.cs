using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service.Interface;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service
{
    public class UserService : BaseService<UserModel>, IUserService
    {
        private readonly IUserRepository iUserRepository;

        public UserService(IUserRepository iUserRepository,IMapper mapper)
        {
            base._iBaseRepository = iUserRepository;
            this.iUserRepository = iUserRepository;
            base.mapper = mapper;
        }

        #region 查询函数


        public override async Task<ApiResponse> SearchAsync(dynamic primaryKey)
        {
            try
            {
                var data = await iUserRepository.SearchAsync(primaryKey);
                var dataDto = mapper.Map<UserDto>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryFirstOrDefaultAsync(Expression<Func<UserModel, bool>> func)
        {
            try
            {
                var data = await iUserRepository.QueryFirstOrDefaultAsync(func);
                var dataDto = mapper.Map<UserDto>(data);
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
                var data = await iUserRepository.QueryAsync();
                var dataDto = mapper.Map<List<UserDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<UserModel, bool>> func)
        {
            try
            {
                var data = await iUserRepository.QueryAsync(func);
                var dataDto = mapper.Map<List<UserDto>>(data);
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
                var data = await iUserRepository.QueryAsync(page, size, total);
                var dataDto = mapper.Map<List<UserDto>>(data);
                return new ApiResponse(true, dataDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public override async Task<ApiResponse> QueryAsync(Expression<Func<UserModel, bool>> func, int page, int size, global::SqlSugar.RefAsync<int> total)
        {
            try
            {
                var data = await iUserRepository.QueryAsync(func, page, size, total);
                var dataDto = mapper.Map<List<UserDto>>(data);
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