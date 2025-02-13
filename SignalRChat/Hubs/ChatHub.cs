using Microsoft.AspNetCore.SignalR;

/*

How It Works in a Web Application
    A client (e.g., browser) connects to the ChatHub.
    The client calls SendMessage(user, message) (from JavaScript or another client).
    The server receives the message and forwards it to all clients using Clients.All.SendAsync("ReceiveMessage", user, message).
    Each client executes a JavaScript function named "ReceiveMessage", displaying the message in the UI.

*/

namespace SignalRChat.Hubs 
//namespace means that things defined within can only be accessed with explicit reffereance to the namespace, preventing naming conflicts
{
    public class ChatHub : Hub
    //A hub is a high-level pipeline that clients can connect to and invoke methods on the server.
    {
        // Task is the type returned, it is ongoing asynchronous operation
        // caller can await its completion
        public async Task SendMessage(string user, string message)
        {
            // Clients.All -> send to all connected clients
            // calls a js function 'ReceiveMessage' on the client using the arguments 'user' and 'message'
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}