using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs
{
	public class GameHub : Hub
	{
		public async Task JoinRoom(string roomCode)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
			await Clients.Group(roomCode).SendAsync("PlayerJoined", Context.UserIdentifier);
		}

		public async Task SendMove(string roomCode, int x, int y)
		{
			await Clients.Group(roomCode).SendAsync("ReceiveMove", Context.UserIdentifier, x, y);
		}
	}
}
