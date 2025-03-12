using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Providers;

public class NameUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.Identity?.Name!;
    }
}