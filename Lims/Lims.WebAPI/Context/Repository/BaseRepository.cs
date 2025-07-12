using Lims.WebAPI.Context.UnitOfWork;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;

namespace Lims.WebAPI.Context.Repository
{
    public class BaseRepository<TEntity> : SimpleClient<TEntity>, IBaseRepository<TEntity> where TEntity : class, new()
    {
        // SqlSugarClient dbClient;
        public BaseRepository(ISqlSugarClient db) : base(db)
        {            
            base.Context =db ;      
        }

        public async Task<bool> CreateAsync(TEntity entity)
        {
            return await base.InsertAsync(entity);
        }

        public async Task<bool> EditAsync(TEntity entity)
        {
            return await base.UpdateAsync(entity);
        }

        /// <summary>
        /// 批量修改数据
        /// </summary>
        /// <param name="updateObjs"></param>
        /// <returns></returns>
        public async Task<bool> EditRangeAsync(List<TEntity> updateObjs)
        {
            return await base.UpdateRangeAsync(updateObjs);
        }


        public async Task<bool> DeleteAsync(string primmaryKey)
        {
            return await base.DeleteByIdAsync(primmaryKey);
        }

        public virtual async Task<TEntity> SearchAsync(string primmaryKey)
        {
            return await base.GetByIdAsync(primmaryKey);
        }

        public virtual async Task<TEntity> QueryFirstOrDefaultAsync(Expression<Func<TEntity, bool>> func)
        {
            return await base.GetFirstAsync(func);
        }

        public virtual async Task<List<TEntity>> QueryAsync()
        {
            return await base.GetListAsync();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> func)
        {
            return await base.IsAnyAsync(func);
        }

        public virtual async Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func)
        {
            return await base.GetListAsync(func);
        }

        public virtual async Task<List<TEntity>> QueryAsync(int page, int size, RefAsync<int> total)
        {
            return await base.Context.Queryable<TEntity>().ToPageListAsync(page, size, total);
        }

        public virtual async Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func, int page, int size, RefAsync<int> total)
        {
            return await base.Context.Queryable<TEntity>()
                .Where(func)
                .ToPageListAsync(page, size, total);
        }

        //public async Task<bool> CreateRangeAsync(TEntity[] entitys)
        //{
        //    return await base.InsertRangeAsync(entitys);
        //}

        //public async Task<bool> DeleteRangeAsync(dynamic[] primmaryKeys)
        //{
        //    return await base.DeleteByIdsAsync(primmaryKeys);
        //}
    }
}