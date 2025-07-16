using Lims.Common.Dtos;
using Lims.Common.Parameters;

namespace Lims.WPF.Services.Interface
{
    public interface IItemService : IBaseService<ItemDto>
    {
        //public Task<ApiResponse<List<ItemDto>>> QueryWithChildObjectsAsync(Expression<Func<ItemDto, bool>> func);

        //public Task<ApiResponse<List<ItemDto>>> QueryWithChildObjectsAsync(Expression<Func<ItemDto, bool>> func, int pageNumber, int pageSize);
        Task<ApiResponse<List<ItemDto?>?>> GetMyItemsAsync(ItemFilterParam param);
        Task<ApiResponse<List<ItemDto?>?>> GetMyItemsBySampleCodeAsync(ItemFilterParam param);
        //Task<ApiResponse<List<ItemDto>>> GetSamplesByTestProgressAsync(ItemFilterParam param);
        Task<ApiResponse<List<ItemDto?>>> GetAllItemsByDateAsync(ItemFilterParam param);
        Task<ApiResponse<List<ItemDto?>>> GetAllItemsByTestProgressAsync(ItemFilterParam param);
        Task<ApiResponse<List<ItemDto?>>> GetAllItemsBySampleCodeAsync(ItemFilterParam param);
        Task<ApiResponse<List<ItemDto?>>> GetAllItemsByMethodStandardIdAsync(ItemFilterParam param);
        Task<ApiResponse<List<ItemDto?>>> GetAllItemsBySampleCodesAsync(ItemFilterParam param);
        Task<ApiResponse<ItemDto?>> GetFirstItemBySampleCodeAndKeyItemAsync(ItemFilterParam param);
    }
}