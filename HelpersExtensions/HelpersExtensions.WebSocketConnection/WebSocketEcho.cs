using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelpersExtensions.WebSocketConnection
{
	static class WebSocketEcho<TWSEvents>
		where TWSEvents : WebSocketEvents
	{
		public static async Task Invoke(HttpContext context, WebSocket webSocket, CancellationToken cancellationToken)
		{
			var wsEvents = context.RequestServices.GetRequiredService<TWSEvents>();
			var wsClient = (WebSocketClient<TWSEvents>)context.RequestServices.GetRequiredService<IWebSocketClient<TWSEvents>>();

			var clientId = Guid.NewGuid().ToString();
			wsClient.Connections.Add(clientId, webSocket);
			await wsEvents.OnClientConnected(clientId, cancellationToken);

			var buffer = new byte[1024 * 4];
			var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

			while (!result.CloseStatus.HasValue)
			{
				var mensagemRecebida = Encoding.UTF8.GetString(buffer, 0, result.Count);
				await wsEvents.OnReceive(mensagemRecebida, clientId, cancellationToken);

				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}

			await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
			wsClient.Connections.Remove(clientId);
			await wsEvents.OnClientDisconnected(clientId, CancellationToken.None);
		}
	}
}
