using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace _001TN0172
{
    public class CloseBroswer : Hub
    {
        // This will track whether the client disconnected due to browser close
        private static Dictionary<string, bool> _clientDisconnectFlags = new Dictionary<string, bool>();

        // This method will be invoked explicitly when the client is closing the browser tab
        // This method will be invoked when the client sends the "PageExit" message
        public async Task PageExit(string message)
        {
            // Handle the page exit event here
            // For example, log the message or perform some server-side cleanup
            Console.WriteLine($"Received page exit message: {message}");

            // You could update user status, cleanup data, or perform other actions
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        // Override OnDisconnectedAsync to catch when the client disconnects
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_clientDisconnectFlags.ContainsKey(Context.ConnectionId) && _clientDisconnectFlags[Context.ConnectionId])
            {
                // The client explicitly notified us that it is closing the browser
                Console.WriteLine("The browser window/tab was closed for client " + Context.ConnectionId);
            }
            else
            {
                // This will run when the client disconnects for any reason (including network issues)
                Console.WriteLine("The client disconnected due to some other means (e.g., network error).");
            }

            // Remove the flag as the connection has been disconnected
            if (_clientDisconnectFlags.ContainsKey(Context.ConnectionId))
            {
                _clientDisconnectFlags.Remove(Context.ConnectionId);
            }

            // Call the base method to ensure proper cleanup
            await base.OnDisconnectedAsync(exception);
        }
    }
}
