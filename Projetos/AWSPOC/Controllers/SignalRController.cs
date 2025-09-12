using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("[controller]")]
public class SignalRController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IHubContext<HubSignalR> _hubContext;

    public SignalRController(IConfiguration config, IHubContext<HubSignalR> hubContext)
    {
        _config = config;
        _hubContext = hubContext;
    }

    [HttpPost("publish")]
    public async Task<IActionResult> Publish([FromBody] HubMessage body)
    {
        // Envia para todos os clientes conectados no Hub
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", body.User, body.Message);
        return Ok(new { status = "mensagem enviada" });
    }
}
