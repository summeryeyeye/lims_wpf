using AutoMapper;
using Lims.Common.Dtos;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service;
using Lims.WebAPI.Service.Interface;

namespace Lims.WebAPI.Controllers
{
    public class ReagentsController : MyBaseController<ReagentModel, ReagentDto>
    {
        private readonly IReagentService reagentService;

        public ReagentsController(IMapper mapper, IReagentService reagentService) : base(mapper, (BaseService<ReagentModel>)reagentService)
        {
            this.reagentService = reagentService;
        }
    }
}
