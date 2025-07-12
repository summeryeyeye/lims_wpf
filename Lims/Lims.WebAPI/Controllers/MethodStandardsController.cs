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
    public class MethodStandardsController : MyBaseController<MethodStandardModel, MethodStandardDto>
    {
        private readonly IMethodStandardService methodStandardService;
        private readonly IMapper mapper;

        public MethodStandardsController(IMethodStandardService methodStandardService, IMapper mapper) : base(mapper, (BaseService<MethodStandardModel>)methodStandardService)
        {
            this.methodStandardService = methodStandardService;
            this.mapper = mapper;
        }     


        [HttpGet]
        public async Task<ApiResponse> GetMethodStandardsBySearchWord([FromQuery] MethodStandardFilterParam param) => await methodStandardService.QueryAsync(m => m.SampleState == param.SampleState && (m.TestItem.Contains(param.SearchWord) || m.TestMethod.Contains(param.SearchWord)));
        [HttpGet]
        public async Task<ApiResponse> GetMethodStandardsByTestItem([FromQuery] MethodStandardFilterParam param) => await methodStandardService.QueryAsync(m => m.SampleState == param.SampleState && m.TestItem == param.TestItem);
        [HttpGet]
        public async Task<ApiResponse> GetMethodStandardsByKeyItem([FromQuery] MethodStandardFilterParam param) => await methodStandardService.QueryAsync(m => m.SampleState == param.SampleState && m.KeyItem.Contains(param.KeyItem));
        [HttpGet]
        public async Task<ApiResponse> GetMethodStandardsByTester([FromQuery] MethodStandardFilterParam param) => await methodStandardService.QueryAsync(m => m.Tester == param.TesterName || m.Tester == param.TesterGroup);
    }
}
