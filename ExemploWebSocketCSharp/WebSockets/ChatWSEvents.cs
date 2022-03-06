using HelpersExtensions.WebSocketConnection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExemploWebSocketCSharp.WebSockets
{
	class ChatWSEvents : WebSocketEvents
	{
		private readonly IWebSocketClient<ChatWSEvents> webSocketClient;

		public ChatWSEvents(IWebSocketClient<ChatWSEvents> webSocketClient)
		{
			this.webSocketClient = webSocketClient;
		}

		protected override async Task OnClientConnected(string clientId, CancellationToken cancellationToken)
		{
			Console.WriteLine($"'{clientId}' conectou");
			await webSocketClient.SendToAllExcept($"'</b>{clientId}<b>' conectou", new[] { clientId }, cancellationToken);
		}

		protected override async Task OnClientDisconnected(string clientId, CancellationToken cancellationToken)
		{
			Console.WriteLine($"'{clientId}' desconectou");
			await webSocketClient.SendToAll($"'</b>{clientId}<b>' desconectou", cancellationToken);
		}

		protected override async Task OnReceive(string message, string clientId, CancellationToken cancellationToken)
		{
			Console.WriteLine($"'{clientId}': {message}");
			await webSocketClient.SendToAll($"'</b>{clientId}<b>': {message}", cancellationToken);
		}
	}
}