using Lims.Common;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.WPF.Services.Interface;

namespace Lims.WPF.Services.Services
{
    public class SubItemStandardService : BaseService<SubItemStandardDto>, ISubItemStandardService
    {
        public SubItemStandardService() : base("SubItemStandards")
        {
        }

        public async Task<ApiResponse<bool>> GetAnySubItemStandardsByKeyWordAsync(SubItemStandardFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetAnySubItemStandardsByKeyWord?Name={param.ParentNames}",
            };
            return await client.ExecuteAsync<bool>(request);
        }

        public async Task<ApiResponse<List<SubItemStandardDto>>> GetSubItemStandardsByTestItemAsync(SubItemStandardFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetSubItemStandardsByTestItem?ParentNames={param.ParentNames}",
            };
            return await client.ExecuteAsync<List<SubItemStandardDto>>(request);
        }
        public async Task<ApiResponse<List<SubItemStandardDto>>> GetSubItemStandardsBySubItemAsync(SubItemStandardFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetSubItemStandardsBySubItem?Name={param.Name}",
            };
            return await client.ExecuteAsync<List<SubItemStandardDto>>(request);
        }
    }
}