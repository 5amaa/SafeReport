using Microsoft.AspNetCore.SignalR.Client;
using SafeReport.Web.DTOs;
namespace SafeReport.Web.Services
{
    public class NotificationService
    {
        private HubConnection? _hubConnection;
        public event Action<ReportDTO>? OnNewReport;

        public async Task StartAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/reportHub") 
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<ReportDTO>("ReceiveNewReport", (report) =>
            {
                OnNewReport?.Invoke(report);
            });

            await _hubConnection.StartAsync();
        }
    }
}
