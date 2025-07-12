using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using SqlSugar;

namespace Lims.WebAPI.Context.Repository
{
    public class LoggerRepository : BaseRepository<LoggerModel>, ILoggerRepository
    {
        public LoggerRepository(ISqlSugarClient db) : base(db)
        {
        }
    }
}