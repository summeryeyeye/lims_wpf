using AutoMapper;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service;
using Lims.WebAPI.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Lims.WebAPI.Controllers
{
    public class ParallelTestingsController : MyBaseController<ParallelTestingModel, ParallelTestingDto>
    {
        private readonly IParallelTestingService _parallelTestingService;
        public ParallelTestingsController(IMapper mapper, IParallelTestingService parallelTestingService) : base(mapper, (BaseService<ParallelTestingModel>)parallelTestingService)
        {
            this._parallelTestingService = parallelTestingService;
        }
        //[HttpGet]
        //public async Task<ApiResponse> GetParallelTestingsByParentId([FromQuery] ParallelTestingFilterParam param)
        //{
        //    return await _parallelTestingService.GetParallelTestingsByParentIdAsync(p => p.ParentId == param.ParentId);
        //}
    }
}
