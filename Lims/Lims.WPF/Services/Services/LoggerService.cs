using Lims.Common;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.WPF.Services.Interface;

namespace Lims.WPF.Services.Services
{
    public class LoggerService : BaseService<LoggerDto>, ILoggerService
    {
        public LoggerService() : base("Loggers")
        {
        }

        public async Task<ApiResponse<List<LoggerDto>>> GetLoggersByFilterAsync(LoggerFilterParam param)
        {
            BaseRequest request = new()
            {
                Method = RestSharp.Method.GET,
                Route = @$"api/{serviceName}/GetLoggersByFilter?ReceiverName={param.ReceiverName}&LogLevel={param.LogLevel}",
            };
            return await client.ExecuteAsync<List<LoggerDto>>(request);
        }
    }
}