using Lims.Common.Dtos;
using Lims.Common.Parameters;

namespace Lims.WPF.Services.Interface
{
    public interface ILoggerService : IBaseService<LoggerDto>
    {
        Task<ApiResponse<List<LoggerDto>>> GetLoggersByFilterAsync(LoggerFilterParam param);
    }
}