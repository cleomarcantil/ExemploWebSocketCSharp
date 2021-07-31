using HelpersExtensions.WebSocketConnection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExemploWebSocketCSharp.Services
{
	class WebSocketServerEventsService : IWebSocketServerEvents, IDisposable
	{
		private readonly IWebSocketClient webSocketClient;

		private readonly Timer _timer;


		public WebSocketServerEventsService(IWebSocketClient webSocketClient)
		{
			this.webSocketClient = webSocketClient;
			_timer = new Timer(async (x) => await webSocketClient.SendToAll($"Do servidor: {DateTime.Now:HH:mm}"), null, 1000, 60_000);
			
		}

		public void Dispose() => _timer.Dispose();

		public async Task OnClientConnected(string clientId)
		{
			Console.WriteLine($"{clientId} conectado");

			await webSocketClient.SendToAllExcept($"{clientId} conectado", new[] { clientId });
		}

		public async Task OnClientDisconnected(string clientId)
		{
			Console.WriteLine($"{clientId} desconectado");

			await webSocketClient.SendToAll($"{clientId} desconectado");
		}

		public async Task OnReceiveMessage(string msg, string clientId)
		{
			Console.WriteLine($"Mensagem de {clientId}: {msg}");
			await webSocketClient.SendToAll($"De {clientId}: {msg}");
		}
	}
}
