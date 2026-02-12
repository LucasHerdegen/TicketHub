using Microsoft.AspNetCore.SignalR;

namespace TicketHub.API.Hubs
{
    public class UserHub : Hub
    {
        private static int _totalViews = 0;

        public override async Task OnConnectedAsync()
        {
            int count = Interlocked.Increment(ref _totalViews);

            await Clients.All.SendAsync("TotalViews", count);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            int count = Interlocked.Decrement(ref _totalViews);

            await Clients.All.SendAsync("TotalViews", count);
            await base.OnDisconnectedAsync(exception);
        }
    }
}