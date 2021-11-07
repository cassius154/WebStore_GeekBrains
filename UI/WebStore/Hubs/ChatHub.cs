using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WebStore.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients/*.All*/.Others.SendAsync("MessageFromClient", message);
        }
    }
}
