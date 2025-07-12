using Lims.Common;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.WPF.Services.Interface;

namespace Lims.WPF.Services.Services
{
    public class SubItemService : BaseService<SubItemDto>, ISubItemService
    {
        public SubItemService() : base("SubItems")
        {
        }

        public async Task<ApiResponse<List<SubItemDto>>> GetSubItemByItemIdAsync(SubItemFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetSubItemByItemId?ItemId={param.ItemId}",
            };
            return await client.ExecuteAsync<List<SubItemDto>>(request);
        }
    }
}