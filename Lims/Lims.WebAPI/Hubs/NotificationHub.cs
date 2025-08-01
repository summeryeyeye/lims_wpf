using Lims.Common.Dtos;
using Lims.WebAPI.Singleton;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Lims.WebAPI.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;
        //private readonly ILoggerMessage
        private readonly ITaskStatusService _taskStatusService;

        public NotificationHub(ITaskStatusService taskStatusService, ILogger<NotificationHub> logger)
        {

            this._taskStatusService = taskStatusService;
            this._logger = logger;
        }
        public override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"客户端已连接: {connectionId}", connectionId);
            return base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"客户端断开连接: {connectionId}", connectionId);
            // 从所有组中移除
            await Groups.RemoveFromGroupAsync(connectionId, "TaskStatusChanges");
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SubscribeToTaskChanges()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "TaskStatusChanges");
            // 获取当前状态并立即发送
            var currentStatus = await _taskStatusService.GetCurrentTaskStatusAsync();
            await Clients.Caller.SendAsync("ReceiveInitialTaskStatus", currentStatus);

        }



    }
}

