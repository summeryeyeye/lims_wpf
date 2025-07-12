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
    public class SubItemsController : MyBaseController<SubItemModel, SubItemDto>
    {
        private readonly ISubItemService subItemService;
        private readonly IMapper mapper;

        public SubItemsController(ISubItemService subItemService, IMapper mapper) : base(mapper, (BaseService<SubItemModel>)subItemService)
        {
            this.subItemService = subItemService;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ApiResponse> GetSubItemByItemId([FromQuery] SubItemFilterParam param) => await subItemService.QueryAsync(s => s.ItemId == param.ItemId);
    }
}
