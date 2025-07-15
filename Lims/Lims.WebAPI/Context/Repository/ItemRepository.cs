using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using System.Linq.Expressions;

namespace Lims.WebAPI.Context.Repository
{
    public class ItemRepository : BaseRepository<ItemModel>, IItemRepository
    {
        public ItemRepository(SqlSugar.ISqlSugarClient db) : base(db)
        {
        }

        /// <summary>
        /// 通过项目获取样品
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<List<ItemModel>> QuerySampleOfItemAsync(Expression<Func<ItemModel, bool>> func)
        {
            var queryble = base.Context.Queryable<ItemModel>();
            queryble = queryble.Includes(i => i.Sample, s => s.Items.ToList());
            return await queryble.Where(func).ToListAsync();
        }

        /// <summary>
        /// 联表查找
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<List<ItemModel>> RelativeQueryAsync(Expression<Func<ItemModel, bool>> func)
        {
            var queryble = base.Context.Queryable<ItemModel>();
            //if (parm.WithMethod)
            queryble = queryble.Includes(i => i.MethodStandard);
            //if (parm.WithPruduct)
            queryble = queryble.Includes(i => i.ProductStandard);
           
            queryble = queryble.Includes(i => i.Sample, s => s.Items.ToList());
            queryble = queryble.Includes(i => i.SubItems.OrderBy(i => i.SubItemId).ToList());
            return await queryble.Where(func).ToListAsync();
        }

        public async override Task<ItemModel> QueryFirstOrDefaultAsync(Expression<Func<ItemModel, bool>> func)
        {
            var queryble = base.Context.Queryable<ItemModel>();
            queryble = queryble.Includes(i => i.MethodStandard);
            return await queryble.Where(func).FirstAsync();
            //return base.QueryFirstOrDefaultAsync(func);
        }




        /// <summary>
        /// 分页联表查找
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parm"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<ItemModel>> RelativeQueryAsync(Expression<Func<ItemModel, bool>> func, int pageNumber, int pageSize)
        {
            var queryble = base.Context.Queryable<ItemModel>();
            //if (parm.WithMethod)
            queryble = queryble.Includes(i => i.MethodStandard);
            //if (parm.WithPruduct)
            queryble = queryble.Includes(i => i.ProductStandard);
            
            queryble = queryble.Includes(i => i.Sample, s => s.Items.ToList());
            queryble = queryble.Includes(i => i.SubItems.OrderBy(i => i.SubItemId).ToList());
            return await queryble.Where(func).ToPageListAsync(pageNumber, pageSize);
        }
    }
}