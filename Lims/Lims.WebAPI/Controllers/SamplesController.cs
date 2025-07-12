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
    public class SamplesController : MyBaseController<SampleModel, SampleDto>
    {
        private readonly ISampleService sampleService;

        public SamplesController(ISampleService sampleService, IMapper mapper) : base(mapper, (BaseService<SampleModel>)sampleService)
        {
            this.sampleService = sampleService;
        }

        [HttpGet]
        public async Task<ApiResponse> GetSamples([FromQuery] SampleFilterParam param)
        {

            //if (param.IsShowCompeleteDatas)
            return param.WithItems ? await sampleService.QueryWithChildObjectsAsync(s => s.CreateTime >= param.MinDate && s.CreateTime <= param.MaxDate) : await sampleService.QueryAsync(s => s.CreateTime >= param.MinDate && s.CreateTime <= param.MaxDate);

            //switch (param.Operation)
            //{
            //    case Operation.Lower:
            //        return param.WithItems ? await sampleService.QueryWithChildObjectsAsync(s => s.Items.Min(i => i.TestProgress) < param.RelativeProgress) : await sampleService.QueryAsync(s => s.Items.Min(i => i.TestProgress) < param.RelativeProgress);
            //    case Operation.Equal:
            //        return param.WithItems ? await sampleService.QueryWithChildObjectsAsync(s => s.Items.Min(i => i.TestProgress) == param.RelativeProgress || s.Items.Max(i => i.TestProgress) == param.RelativeProgress) : await sampleService.QueryAsync(s => s.Items.Min(i => i.TestProgress) == param.RelativeProgress || s.Items.Max(i => i.TestProgress) == param.RelativeProgress);
            //    case Operation.Higher:
            //        return param.WithItems ? await sampleService.QueryWithChildObjectsAsync(s => s.Items.Max(i => i.TestProgress) > param.RelativeProgress) : await sampleService.QueryAsync(s => s.Items.Max(i => i.TestProgress) > param.RelativeProgress);
            //    default:
            //        break;
            //}

            //return param.WithItems ? await sampleService.QueryWithChildObjectsAsync(s => s.Items.Min(i => i.TestProgress) == param.RelativeProgress || s.Items.Max(i => i.TestProgress) == param.RelativeProgress) : await sampleService.QueryAsync(s => s.Items.Min(i => i.TestProgress) == param.RelativeProgress || s.Items.Max(i => i.TestProgress) == param.RelativeProgress);
        }
        [HttpGet]
        public async Task<ApiResponse> GetSamplesBySampleCodeKeyWord([FromQuery] SampleFilterParam param) => await sampleService.QueryWithChildObjectsAsync(s => s.SampleCode.Contains(param.SampleCodeKeyWord));

      
    }
}
