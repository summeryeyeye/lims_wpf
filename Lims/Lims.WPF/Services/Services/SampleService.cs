using Lims.Common;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.WPF.Services.Interface;

namespace Lims.WPF.Services.Services
{
    public class SampleService : BaseService<SampleDto>, ISampleService
    {
        public SampleService() : base("Samples")
        {
        }
        public async Task<ApiResponse<List<SampleDto>>> GetSamplesAsync(SampleFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetSamples?MinDate={param.MinDate}&MaxDate={param.MaxDate}&WithItems={param.WithItems}",
            };
            return await client.ExecuteAsync<List<SampleDto>>(request);
        }

        public async Task<ApiResponse<List<SampleDto>>> GetSamplesBySampleCodeKeyWordAsync(SampleFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetSamplesBySampleCodeKeyWord?SampleCodeKeyWord={param.SampleCodeKeyWord}",
            };
            return await client.ExecuteAsync<List<SampleDto>>(request);
        }

        public async Task<ApiResponse<List<SampleDto>>> GetSamplesBySampleCodesAsync(SampleFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetSamplesBySampleCodes?SampleCodes={param.SampleCodes}",
            };
            return await client.ExecuteAsync<List<SampleDto>>(request);
        }
    }
}