using AutoMapper;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service;
using Lims.WebAPI.Service.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lims.WebAPI.Controllers
{
    public class SubItemStandardsController : MyBaseController<SubItemStandardModel, SubItemStandardDto>
    {
        private readonly ISubItemStandardService subItemStandardService;
        private readonly IMapper mapper;

        public SubItemStandardsController(ISubItemStandardService subItemStandardService, IMapper mapper) : base(mapper, (BaseService<SubItemStandardModel>)subItemStandardService)
        {
            this.subItemStandardService = subItemStandardService;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ApiResponse> GetSubItemStandardsByTestItem([FromQuery] SubItemStandardFilterParam param) => await subItemStandardService.QueryAsync(s => s.ParentNames.Contains(param.ParentNames));
        [HttpGet]
        public async Task<ApiResponse> GetSubItemStandardsBySubItem([FromQuery] SubItemStandardFilterParam param) => await subItemStandardService.QueryAsync(s => s.SubitemName.Equals(param.Name));




        [HttpGet]
        public async Task<ApiResponse> GetAnySubItemStandardsByKeyWord([FromQuery] SubItemStandardFilterParam param) => await subItemStandardService.AnyAsync(s => s.ParentNames == param.ParentNames);
    }
}
