using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using SqlSugar;

namespace Lims.WebAPI.Context.Repository
{
    public class MethodStandardRepository : BaseRepository<MethodStandardModel>, IMethodStandardRepository
    {
        public MethodStandardRepository(ISqlSugarClient db) : base(db)
        {
           
        }

        public async Task<MethodStandardModel> SearchAsync(int primmaryKey)
        {
            return await base.GetByIdAsync(primmaryKey);
        }
    }
}
