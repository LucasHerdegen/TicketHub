using Microsoft.AspNetCore.SignalR;

namespace TicketHub.API.Hubs
{
    public class UserHub : Hub
    {
        private static int _totalViews = 0;
        private static int _activeViews = 0;

        public override async Task OnConnectedAsync()
        {
            int totalViews = Interlocked.Increment(ref _totalViews);
            int activeViews = Interlocked.Increment(ref _activeViews);

            await Clients.All.SendAsync("TotalViews", totalViews, activeViews);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            int totalViews = _totalViews;
            int activeViews = Interlocked.Decrement(ref _activeViews);

            await Clients.All.SendAsync("TotalViews", totalViews, activeViews);
            await base.OnDisconnectedAsync(exception);
        }
    }
}