using Lims.Common.Dtos;
using Lims.WebAPI.Singleton;
using Microsoft.AspNetCore.SignalR;

namespace Lims.WebAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly TaskCount _taskCount;

        public ChatHub(TaskCount taskCount)
        {
            this._taskCount = taskCount;
        }
        public override  Task OnConnectedAsync()
        {
            Console.WriteLine($"ID:{Context.ConnectionId} 已连接");
            _taskCount.ListenPostgresql();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"ID:{Context.ConnectionId} 已断开");
            return base.OnDisconnectedAsync(exception);
        }




    }
}
