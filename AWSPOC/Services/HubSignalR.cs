using Microsoft.AspNetCore.SignalR;

public class HubSignalR : Hub
{
    // Opcional: método que os clientes podem chamar
    public async Task SendToAll(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}

