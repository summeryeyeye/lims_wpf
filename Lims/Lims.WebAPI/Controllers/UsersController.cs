using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service;
using Lims.WebAPI.Service.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lims.WebAPI.Controllers
{
    public class UsersController : MyBaseController<UserModel, UserDto>
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public UsersController(IUserService userService, IMapper mapper):base(mapper, (BaseService<UserModel>)userService)
        {
            this.userService = userService;
            this.mapper = mapper;
        }
    }
}
