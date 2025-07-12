using AutoMapper;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.WebAPI.Models;
using Lims.WebAPI.Service;
using Lims.WebAPI.Service.Interface;
using Microsoft.AspNetCore.Mvc;
namespace Lims.WebAPI.Controllers
{
    public class ItemsController : MyBaseController<ItemModel, ItemDto>
    {
        private readonly IItemService itemService;

        public ItemsController(IMapper mapper, IItemService itemService) : base(mapper, (BaseService<ItemModel>)itemService)
        {
            this.itemService = itemService;
        }

        /// <summary>
        /// 获取我的项目,关联检测进度,用户名
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> GetMyItems([FromQuery] ItemFilterParam param)
        {
            switch (param.Operation)
            {
                case Operation.Lower:
                    return await itemService.RelativeQueryAsync(i => i.Tester == param.Tester && i.TestProgress < param.TestProgress);
                case Operation.Equal:
                    var response = await itemService.RelativeQueryAsync(i => i.Tester == param.Tester && i.TestProgress == param.TestProgress);
                    return response;
                case Operation.Higher:
                    return await itemService.RelativeQueryAsync(i => i.Tester == param.Tester && i.TestProgress > param.TestProgress && i.ResultSubmitTime >= param.MinDate && i.ResultSubmitTime <= param.MaxDate);
                default:
                    break;
            }
            return new ApiResponse("缺少操作符!");
        }
        /// <summary>
        /// 获取我的项目,关联检测进度,用户名，样品编号
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> GetMyItemsBySampleCode([FromQuery] ItemFilterParam param)
        {
            switch (param.Operation)
            {
                case Operation.Lower:
                    return await itemService.RelativeQueryAsync(i => i.SampleCode == param.SampleCode && i.Tester == param.Tester && i.TestProgress < param.TestProgress);
                case Operation.Equal:
                    var response = await itemService.RelativeQueryAsync(i => i.SampleCode == param.SampleCode && i.Tester == param.Tester && i.TestProgress == param.TestProgress);
                    return response;
                case Operation.Higher:
                    return await itemService.RelativeQueryAsync(i => i.SampleCode == param.SampleCode && i.Tester == param.Tester && i.TestProgress > param.TestProgress && i.ResultSubmitTime >= param.MinDate && i.ResultSubmitTime <= param.MaxDate);
                default:
                    break;
            }
            return new ApiResponse("缺少操作符!");
        }




        /// <summary>
        /// 根据日期获取所有项目
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> GetAllItemsByDate([FromQuery] ItemFilterParam param) => await itemService.RelativeQueryAsync(i => i.AppointTime >= param.MinDate && i.AppointTime <= param.MaxDate);
        /// <summary>
        /// 根据样品编号获取相关项目
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> GetAllItemsBySampleCode([FromQuery] ItemFilterParam param) => await itemService.RelativeQueryAsync(i => i.SampleCode == param.SampleCode);
        /// <summary>
        /// 根据项目检测进度获取相关项目
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> GetAllItemsByTestProgress([FromQuery] ItemFilterParam param)
        {
            switch (param.Operation)
            {
                case Operation.Higher:
                    return await itemService.RelativeQueryAsync(i => i.TestProgress > param.TestProgress);
                case Operation.Lower:
                    return await itemService.RelativeQueryAsync(i => i.TestProgress < param.TestProgress);
                case Operation.Equal:
                    return await itemService.RelativeQueryAsync(i => i.TestProgress == param.TestProgress);
            }
            return new ApiResponse("缺少操作符!");
        }
        /// <summary>
        /// 根据方法Id获取相关项目
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> GetAllItemsByMethodStandardId([FromQuery] ItemFilterParam param) => await itemService.RelativeQueryAsync(i => i.MethodStandardId == param.MethodStandardId);

        [HttpGet]
        public async Task<ApiResponse> GetAllItemsBySampleCodes([FromQuery] ItemFilterParam param)
        {
            var sampleCodes = param.SampleCodes.Split("and");
            return await itemService.RelativeQueryAsync(i => sampleCodes.Contains(i.SampleCode));
        }



        /// <summary>
        /// 查询同样品中符合关键词的第一个项目
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse> GetFirstItemBySampleCodeAndKeyWord([FromQuery] ItemFilterParam param) => await itemService.QueryFirstOrDefaultAsync(i => i.SampleCode == param.SampleCode && (i.TestItem.Contains(param.TestItemKeyWord_1) || i.TestItem.Contains(param.TestItemKeyWord_2)));
    }
}
