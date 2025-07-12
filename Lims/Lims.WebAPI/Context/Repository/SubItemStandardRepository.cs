using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using SqlSugar;

namespace Lims.WebAPI.Context.Repository
{
    public class SubItemStandardRepository : BaseRepository<SubItemStandardModel>, ISubItemStandardRepository
    {
        public SubItemStandardRepository(ISqlSugarClient db) : base(db)
        {
        }
    }
}