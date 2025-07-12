using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using SqlSugar;

namespace Lims.WebAPI.Context.Repository
{
    public class ReagentRepository : BaseRepository<ReagentModel>, IReagentRepository
    {
        public ReagentRepository(ISqlSugarClient db) : base(db)
        {
        }
    }
}
