using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using SqlSugar;

namespace Lims.WebAPI.Context.Repository
{
    public class ParallelTestingRepository : BaseRepository<ParallelTestingModel>, IParallelTestingRepository
    {
        public ParallelTestingRepository(ISqlSugarClient db) : base(db)
        {
        }
    }
}
