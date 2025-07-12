using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using SqlSugar;

namespace Lims.WebAPI.Context.Repository
{
    public class ProductStandardRepository : BaseRepository<ProductStandardModel>, IProductStandardRepository
    {
        public ProductStandardRepository(ISqlSugarClient db) : base(db)
        {
        }
    }
}
