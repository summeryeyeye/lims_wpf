using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Models;
using SqlSugar;

namespace Lims.WebAPI.Context.Repository
{
    public class UserRepository : BaseRepository<UserModel>, IUserRepository
    {
        public UserRepository(ISqlSugarClient db) : base(db)
        {
        }
    }
}