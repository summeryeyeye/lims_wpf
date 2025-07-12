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

    public class LoggersController : MyBaseController<LoggerModel, LoggerDto>
    {
        private readonly ILoggerService loggerService;

        public LoggersController(IMapper mapper, ILoggerService loggerService) : base(mapper, (BaseService<LoggerModel>)loggerService)
        {
            this.loggerService = loggerService;
        }

        [HttpGet]
        public async Task<ApiResponse> GetLoggersByFilter([FromQuery] LoggerFilterParam param) => await loggerService.QueryAsync(l => l.ReceiverName == param.ReceiverName && (int)l.LogLevel == param.LogLevel);
    }
}
