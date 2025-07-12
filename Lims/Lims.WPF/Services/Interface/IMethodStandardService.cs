using Lims.Common.Dtos;
using Lims.Common.Parameters;

namespace Lims.WPF.Services.Interface
{
    public interface IMethodStandardService : IBaseService<MethodStandardDto>
    {
        Task<ApiResponse<List<MethodStandardDto>>> GetMethodStandardsBySearchWordAsync(MethodStandardFilterParam param);
        Task<ApiResponse<List<MethodStandardDto>>> GetMethodStandardsByTestItemAsync(MethodStandardFilterParam param);
        Task<ApiResponse<List<MethodStandardDto>>> GetMethodStandardsByTesterAsync(MethodStandardFilterParam param);
        Task<ApiResponse<List<MethodStandardDto>>> GetMethodStandardsByKeyItemAsync(MethodStandardFilterParam param);

    }
}
