using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using SqlSugar;
using System.Linq.Expressions;

namespace Lims.WebAPI.Context.Repository
{
    public class SampleRepository : BaseRepository<SampleModel>, ISampleRepository
    {
        public SampleRepository(ISqlSugarClient db) : base(db)
        {
        }

        public async Task<List<SampleModel>> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func1, Expression<Func<SampleModel, List<ItemModel>>> func2, int pageNumber, int pageSize, RefAsync<int> total, string orderFileds)
        {
            return await base.Context.Queryable<SampleModel>()
             .Where(func1)
             .Includes(func2, i => i.SubItems).OrderBy(orderFileds)
             .ToPageListAsync(pageNumber, pageSize, total);
        }

        public async Task<List<SampleModel>> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func1, Expression<Func<SampleModel, List<ItemModel>>> func2, string orderFileds)
        {
            return await base.Context.Queryable<SampleModel>()
             .Where(func1)
             .Includes(func2).OrderBy(orderFileds)
             .ToListAsync();
        }

        public async Task<List<SampleModel>> QueryWithChildObjectsAsync(Expression<Func<SampleModel, List<ItemModel>>> func, int pageNumber, int pageSize, RefAsync<int> total)
        {
            return await base.Context.Queryable<SampleModel>()
             .Includes(func, i => i.SubItems)
             .ToPageListAsync(pageNumber, pageSize, total);
        }

        public async Task<List<SampleModel>> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func, string orderFileds)
        {
            return await base.Context.Queryable<SampleModel>()
             .Includes(s => s.Items.ToList())
             .Where(func)
             .OrderBy(orderFileds)
             .ToListAsync();
        }

        public async Task<List<SampleModel>> QueryWithChildObjectsAsync(Expression<Func<SampleModel, bool>> func, int pageNumber, int pageSize)
        {
            return await base.Context.Queryable<SampleModel>()
             .Where(func)
             .ToPageListAsync(pageNumber, pageSize);
        }
    }
}