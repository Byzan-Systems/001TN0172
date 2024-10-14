using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC
{
    public class UploadProgressHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var sessionId = Context.GetHttpContext().Session.Id;
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var sessionId = Context.GetHttpContext().Session.Id;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
