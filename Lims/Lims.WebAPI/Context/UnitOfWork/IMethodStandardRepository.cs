using Lims.WebAPI.Models;

namespace Lims.WebAPI.Context.UnitOfWork
{
    public interface IMethodStandardRepository : IBaseRepository<MethodStandardModel>
    {
        public Task<MethodStandardModel> SearchAsync(int primaryKey);
    }
}
