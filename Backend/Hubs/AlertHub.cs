using System;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs;

public class AlertHub: Hub
{
    public async Task SendAlert(string message)
    {
        await Clients.All.SendAsync("ReceiveAlert", message);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception); 
    }

}
