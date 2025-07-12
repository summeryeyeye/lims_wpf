using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using SqlSugar;

namespace Lims.WebAPI.Context.Repository
{
    public class SubItemRepository : BaseRepository<SubItemModel>, ISubItemRepository
    {
        public SubItemRepository(ISqlSugarClient db) : base(db)
        {
        }
    }
}