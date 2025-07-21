using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using SqlSugar;

namespace Lims.WebAPI.Context.Repository
{
    public class ParalleltestingRepository : BaseRepository<ParalleltestingModel>, IParalleltestingRepository
    {
        public ParalleltestingRepository(ISqlSugarClient db) : base(db)
        {
        }
    }
}
