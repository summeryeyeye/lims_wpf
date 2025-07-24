using Lims.WebAPI.Models;
using System.Linq.Expressions;

namespace Lims.WebAPI.Service.Interface
{
    public interface IParallelTestingService : IBaseService<ParallelTestingModel>
    {
        Task<ApiResponse> GetParallelTestingsByParentIdAsync(Expression<Func<ParallelTestingModel, bool>> func);
    }
}