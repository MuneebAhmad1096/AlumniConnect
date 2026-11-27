using Microsoft.AspNetCore.SignalR;

namespace AlumniConnect.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendProjectAlert(string title, string domain)
        {
            await Clients.All.SendAsync("ReceiveProjectAlert", title, domain);
        }
    }
}