using Lims.Common;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.WPF.Services.Interface;

namespace Lims.WPF.Services.Services
{
    public class MethodStandardService : BaseService<MethodStandardDto>, IMethodStandardService
    {
        public MethodStandardService() : base("MethodStandards")
        {

        }

        public async Task<ApiResponse<List<MethodStandardDto>>> GetMethodStandardsBySearchWordAsync(MethodStandardFilterParam param)
        {
            BaseRequest request = new BaseRequest()
            {
                Method = RestSharp.Method.GET,
                Route = $"api/{serviceName}/GetMethodStandardsBySearchWord?SampleState={param.SampleState}&SearchWord={param.SearchWord}",
            };
            return await client.ExecuteAsync<List<MethodStandardDto>>(request);
        }

        public async Task<ApiResponse<List<MethodStandardDto>>> GetMethodStandardsByTesterAsync(MethodStandardFilterParam param)
        {
            BaseRequest request = new BaseRequest()
            {
                Method = RestSharp.Method.GET,
                Route = $"api/{serviceName}/GetMethodStandardsByTester?TesterName={param.TesterName}&TesterGroup={param.TesterGroup}"
            };
            return await client.ExecuteAsync<List<MethodStandardDto>>(request);
        }

        public async Task<ApiResponse<List<MethodStandardDto>>> GetMethodStandardsByTestItemAsync(MethodStandardFilterParam param)
        {
            BaseRequest request = new BaseRequest()
            {
                Method = RestSharp.Method.GET,
                Route = $"api/{serviceName}/GetMethodStandardsByTestItem?SampleState={param.SampleState}&TestItem={param.TestItem}"
            };
            return await client.ExecuteAsync<List<MethodStandardDto>>(request);
        }
        public async Task<ApiResponse<List<MethodStandardDto>>> GetMethodStandardsByKeyItemAsync(MethodStandardFilterParam param)
        {
            BaseRequest request = new BaseRequest()
            {
                Method = RestSharp.Method.GET,
                Route = $"api/{serviceName}/GetMethodStandardsByKeyItem?SampleState={param.SampleState}&KeyItem={param.KeyItem}"
            };
            return await client.ExecuteAsync<List<MethodStandardDto>>(request);
        }
    }
}
