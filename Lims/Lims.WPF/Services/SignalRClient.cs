using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lims.WPF.Services
{
    public class SignalRClient : IDisposable
    {
        private HubConnection? _hubConnection;
        private bool _disposed = false;

        public  event Action<string>? OnTaskStatusChanged;
        public event Action<string>? OnInitialDataReceived;
        public event Action<Exception>? OnConnectionError;

        public async Task InitializeAsync(string hubUrl)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect(new RetryPolicy())
                .Build();

            // 设置消息处理器
            _hubConnection.On<string>("ReceiveInitialTaskStatus", HandleInitialTaskData);
            _hubConnection.On<string>("ReceiveTaskChange", HandleTaskChange);

            _hubConnection.Closed += HandleConnectionClosed!;
            _hubConnection.Reconnecting += HandleReconnecting!;
            _hubConnection.Reconnected += HandleReconnected!;

            await StartConnectionAsync();
        }

        private void HandleTaskChange(string data)
        {
            OnTaskStatusChanged?.Invoke(data);
        }

        private void HandleInitialTaskData(string data)
        {
            OnInitialDataReceived?.Invoke(data);
        }

        private Task HandleConnectionClosed(Exception error)
        {
            OnConnectionError?.Invoke(error ?? new Exception("连接已关闭"));
            return Task.CompletedTask;
        }

        private Task HandleReconnecting(Exception error)
        {
            OnConnectionError?.Invoke(new Exception("正在尝试重新连接...", error));
            return Task.CompletedTask;
        }

        private Task HandleReconnected(string connectionId)
        {
            OnConnectionError?.Invoke(new Exception("连接恢复成功"));
            return Task.CompletedTask;
        }

        private async Task StartConnectionAsync()
        {
            try
            {
                await _hubConnection!.StartAsync();
                await _hubConnection.InvokeAsync("SubscribeToTaskChanges");
            }
            catch (Exception ex)
            {
                OnConnectionError?.Invoke(ex);
            }
        }

        private class RetryPolicy : IRetryPolicy
        {
            public TimeSpan? NextRetryDelay(RetryContext retryContext)
            {
                return retryContext.PreviousRetryCount switch
                {
                    0 => TimeSpan.Zero,
                    1 => TimeSpan.FromSeconds(2),
                    2 => TimeSpan.FromSeconds(10),
                    _ => TimeSpan.FromSeconds(30)
                };
            }
        }

        public async Task DisposeAsync()
        {
            if (_disposed) return;

            try
            {
                if (_hubConnection != null)
                {
                    _hubConnection.Closed -= HandleConnectionClosed!;
                    _hubConnection.Reconnecting -= HandleReconnecting!;
                    _hubConnection.Reconnected -= HandleReconnected!;

                    await _hubConnection.StopAsync();
                    await _hubConnection.DisposeAsync();
                }
            }
            finally
            {
                _disposed = true;
            }
        }
        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
