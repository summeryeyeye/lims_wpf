using Lims.Common.Dtos;
using Lims.Common.Parameters;

namespace Lims.WPF.Services.Interface
{
    public interface ISubItemService : IBaseService<SubItemDto>
    {
        Task<ApiResponse<List<SubItemDto>>> GetSubItemByItemIdAsync(SubItemFilterParam param);
    }
}