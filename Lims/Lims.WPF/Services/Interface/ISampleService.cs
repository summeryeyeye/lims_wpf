using Lims.Common.Dtos;
using Lims.Common.Parameters;

namespace Lims.WPF.Services.Interface
{
    public interface ISampleService : IBaseService<SampleDto>
    {
        Task<ApiResponse<List<SampleDto>>> GetSamplesAsync(SampleFilterParam param);
        Task<ApiResponse<List<SampleDto>>> GetSamplesBySampleCodeKeyWordAsync(SampleFilterParam param);
        Task<ApiResponse<List<SampleDto>>> GetSamplesBySampleCodesAsync(SampleFilterParam param);



    }
}