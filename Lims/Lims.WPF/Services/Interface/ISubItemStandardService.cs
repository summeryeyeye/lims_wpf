using Lims.Common.Dtos;
using Lims.Common.Parameters;

namespace Lims.WPF.Services.Interface
{
    public interface ISubItemStandardService : IBaseService<SubItemStandardDto>
    {
        Task<ApiResponse<List<SubItemStandardDto>>> GetSubItemStandardsByTestItemAsync(SubItemStandardFilterParam param);
        Task<ApiResponse<List<SubItemStandardDto>>> GetSubItemStandardsBySubItemAsync(SubItemStandardFilterParam param);
        Task<ApiResponse<bool>> GetAnySubItemStandardsByKeyWordAsync(SubItemStandardFilterParam param);
    }
}