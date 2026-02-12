using Microsoft.AspNetCore.SignalR;

namespace TicketHub.API.Hubs
{
    public class UserHub : Hub
    {
        public int TotalViews { get; set; } = 0;

        public override Task OnConnectedAsync()
        {
            TotalViews++;
            Clients.All.SendAsync("TotalViews", TotalViews);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            TotalViews--;
            Clients.All.SendAsync("TotalViews", TotalViews);
            return base.OnDisconnectedAsync(exception);
        }
    }
}