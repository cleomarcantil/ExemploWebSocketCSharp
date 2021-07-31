using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelpersExtensions.WebSocketConnection
{
	static class WebSocketEcho
	{
		public static async Task Invoke(HttpContext context, WebSocket webSocket)
		{
			var webSocketConnectionManager = (WebSocketConnectionManager)context.RequestServices.GetRequiredService<IWebSocketClient>();
			var webSocketServerEvents = context.RequestServices.GetRequiredService<IWebSocketServerEvents>();

			var clientId = Guid.NewGuid().ToString();
			webSocketConnectionManager.Add(clientId, webSocket);
			await webSocketServerEvents.OnClientConnected(clientId);

			var buffer = new byte[1024 * 4];
			var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

			while (!result.CloseStatus.HasValue)
			{
				var mensagemRecebida = Encoding.UTF8.GetString(buffer, 0, result.Count);
				await webSocketServerEvents.OnReceiveMessage(mensagemRecebida, clientId);

				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}

			await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
			webSocketConnectionManager.Remove(clientId);
			await webSocketServerEvents.OnClientDisconnected(clientId);
		}

	}
}
