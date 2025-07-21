using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service;
using Lims.WebAPI.Service.Interface;

namespace Lims.WebAPI.Controllers
{
    public class ParalleltestingController : MyBaseController<ParalleltestingModel, ParalleltestingDto>
    {
        private readonly IParalleltestingService _iParalleltestingService;
        public ParalleltestingController(IMapper mapper, IParalleltestingService iParalleltestingService) : base(mapper, (BaseService<ParalleltestingModel>)iParalleltestingService)
        {
            this._iParalleltestingService = iParalleltestingService;
        }
    }
}
