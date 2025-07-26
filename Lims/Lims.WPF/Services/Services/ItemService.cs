using Lims.Common;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.WPF.Services.Interface;

namespace Lims.WPF.Services.Services
{
    public class ItemService : BaseService<ItemDto>, IItemService
    {
        public ItemService() : base("Items")
        {
        }


        public async Task<ApiResponse<ObservableCollection<ItemDto>>> GetAllItemsByDateAsync(ItemFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetAllItemsByDate?MinDate={param.MinDate}&MaxDate={param.MaxDate}",
            };
            return await client.ExecuteAsync<ObservableCollection<ItemDto>>(request);
        }
        public async Task<ApiResponse<ObservableCollection<ItemDto>>> GetMyItemsAsync(ItemFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetMyItems?Tester={param.Tester}&TestProgress={param.TestProgress}&Operation={param.Operation}&MinDate={param.MinDate}&MaxDate={param.MaxDate}",
            };
            return await client.ExecuteAsync<ObservableCollection<ItemDto>>(request);
        }

        public async Task<ApiResponse<ObservableCollection<ItemDto>>> GetAllItemsBySampleCodeAsync(ItemFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetAllItemsBySampleCode?SampleCode={param.SampleCode}",
            };
            return await client.ExecuteAsync<ObservableCollection<ItemDto>>(request);
        }
        public async Task<ApiResponse<ObservableCollection<ItemDto>>> GetAllItemsByMethodStandardIdAsync(ItemFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetAllItemsByMethodStandardId?MethodStandardId={param.MethodStandardId}",
            };
            return await client.ExecuteAsync<ObservableCollection<ItemDto>>(request);
        }

        public async Task<ApiResponse<ObservableCollection<ItemDto>>> GetAllItemsBySampleCodesAsync(ItemFilterParam param)
        {
            BaseRequest request = new()
            {
                Parameter = param,
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetAllItemsBySampleCodes?SampleCodes={param.SampleCodes}",
            };
            return await client.ExecuteAsync<ObservableCollection<ItemDto>>(request);
        }

        public async Task<ApiResponse<ItemDto>> GetFirstItemBySampleCodeAndKeyItemAsync(ItemFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetFirstItemBySampleCodeAndKeyItem?SampleCode={param.SampleCode}&KeyItem={param.KeyItem}",
            };
            return await client.ExecuteAsync<ItemDto>(request);
        }
        public async Task<ApiResponse<ObservableCollection<ItemDto>>> GetAllItemsByTestProgressAsync(ItemFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetAllItemsByTestProgress?TestProgress={param.TestProgress}&Operation={param.Operation}",
            };
            return await client.ExecuteAsync<ObservableCollection<ItemDto>>(request);
        }

        public async Task<ApiResponse<ObservableCollection<ItemDto>>> GetMyItemsBySampleCodeAsync(ItemFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetMyItemsBySampleCode?SampleCode={param.SampleCode}&Tester={param.Tester}&TestProgress={param.TestProgress}&Operation={param.Operation}&MinDate={param.MinDate}&MaxDate={param.MaxDate}",
            };
            return await client.ExecuteAsync<ObservableCollection<ItemDto>>(request);
        }

        //public async Task<ApiResponse<List<ItemDto>>> GetSamplesByTestProgressAsync(ItemFilterParam param)
        //{
        //    BaseRequest request = new()
        //    {
        //        Method = RestSharp.Method.GET,
        //        Route = @$"api/{serviceName}/GetSamplesByTestProgress?TestProgress={param.TestProgress}&Operation={param.Operation}",
        //    };
        //    return await client.ExecuteAsync<List<ItemDto>>(request);
        //}
    }
}