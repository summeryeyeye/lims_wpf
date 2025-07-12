using Lims.Common.Dtos;
using Lims.WPF.Services.Interface;

namespace Lims.WPF.Services.Services
{
    public class UserService : BaseService<UserDto>, IUserService
    {
        public UserService() : base("Users")
        {
        }
    }
}