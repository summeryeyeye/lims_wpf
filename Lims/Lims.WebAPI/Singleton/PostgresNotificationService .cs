using Lims.Common.Dtos;
using Lims.WebAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Npgsql;
using SqlSugar;
using System.Data;
using System.Threading.Tasks;
using DbType = SqlSugar.DbType;

namespace Lims.WebAPI.Singleton
{
    public class PostgresNotificationService : BackgroundService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ITaskStatusService _taskStatusService;
        private readonly string _connectionString;
        private readonly ILogger<PostgresNotificationService> _logger; 
        private const int MaxRetryCount = 5;
        private const int RetryDelaySeconds = 10;
        public PostgresNotificationService(IHubContext<NotificationHub> hubContext, IConfiguration configuration, ITaskStatusService taskStatusService, ILogger<PostgresNotificationService> logger)
        {
            this._hubContext = hubContext;
            this._taskStatusService = taskStatusService;
            this._logger = logger;
            _connectionString = configuration.GetConnectionString("POSTGRESQL")!;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        { 
            while (!stoppingToken.IsCancellationRequested)
            {
                int retryCount = 0;
                //await conn.WaitAsync(stoppingToken);
                try
                {
                    await using var conn = new NpgsqlConnection(_connectionString);
                    await conn.OpenAsync(stoppingToken);

                    conn.Notification += async (o, e) =>
                    {
                        var currenTaskStatus = await _taskStatusService.GetCurrentTaskStatusAsync();
                        await _hubContext.Clients.Group("TaskStatusChanges").SendAsync("ReceiveTaskChange", currenTaskStatus);
                    };

                    await using (var cmd = new NpgsqlCommand("LISTEN sample_changed;LISTEN item_changed;LISTEN logger_changed;", conn))
                    {
                        await cmd.ExecuteNonQueryAsync(stoppingToken);
                    }
                    _logger.LogInformation("PostgreSQL 监听服务已启动");
                    retryCount = 0; // 重置重试计数器 // 持续等待通知
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await conn.WaitAsync(stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {

                    _logger.LogInformation("服务正在停止...");
                    throw;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogError(ex, "监听服务发生错误 (重试 {RetryCount}/{MaxRetryCount})",
                        retryCount, MaxRetryCount);

                    if (retryCount >= MaxRetryCount)
                    {
                        _logger.LogCritical("达到最大重试次数，服务将停止");
                        throw;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(RetryDelaySeconds), stoppingToken);
                }
            }
        }
    }
}
