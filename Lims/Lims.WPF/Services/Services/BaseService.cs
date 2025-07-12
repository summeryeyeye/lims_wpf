using Lims.Common;
using Lims.WPF.Services.Interface;
using System.Configuration;

namespace Lims.WPF.Services.Services
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
    {
        protected HttpRestClient client;
        protected string serviceName;

        public BaseService(string serviceName)
        {
            this.serviceName = serviceName;

#if DEBUG
            var serviceRoutePath = ConfigurationManager.AppSettings["ServiceRoutePath"].ToString();
#else
            var serviceRoutePath = ConfigurationManager.AppSettings["ServiceRoutePath"].ToString();
#endif
           client = new HttpRestClient(serviceRoutePath);
           //client = new HttpRestClient(@$"https://localhost:9999/");
        }


        public async Task<ApiResponse<bool>> CreateAsync(TEntity entity)
        {
            BaseRequest request = new BaseRequest()
            {
                Method = RestSharp.Method.POST,
                Route = $"api/{serviceName}/Create",
                Parameter = entity,
            };
            return await client.ExecuteAsync<bool>(request);
        }


        public async Task<ApiResponse<bool>> DeleteAsync(dynamic? primmaryKey)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.DELETE,
                Route = $"api/{serviceName}/Delete/{primmaryKey}",
            };
            return await client.ExecuteAsync<bool>(request);
        }
        public async Task<ApiResponse<bool>> UpdateAsync(TEntity entity)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.POST,
                Route = $"api/{serviceName}/Update",
                Parameter = entity,
            };
            return await client.ExecuteAsync<bool>(request);
        }


        public async Task<ApiResponse<bool>> UpdateRangeAsync(List<TEntity> updateObjs)
        {
            BaseRequest request = new BaseRequest()
            {
                Method = RestSharp.Method.POST,
                Route = $"api/{serviceName}/UpdateRange",
                Parameter = updateObjs,
            };

            return await client.ExecuteAsync<bool>(request);
        }

        public async Task<ApiResponse<List<TEntity>>> GetAllAsync()
        {
            BaseRequest request = new BaseRequest()
            {
                Method = RestSharp.Method.GET,
                Route = $"api/{serviceName}/GetAll",
            };
            return await client.ExecuteAsync<List<TEntity>>(request);
        }


        public async Task<ApiResponse<TEntity>> GetSingleAsync(dynamic? primmaryKey)
        {
            BaseRequest request = new BaseRequest()
            {
                Method = RestSharp.Method.GET,
                Route = $"api/{serviceName}/GetSingle/{primmaryKey}",
            };
            return await client.ExecuteAsync<TEntity>(request);
        }



    }
}