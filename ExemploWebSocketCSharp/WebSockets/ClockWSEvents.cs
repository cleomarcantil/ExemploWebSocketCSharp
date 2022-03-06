using HelpersExtensions.WebSocketConnection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExemploWebSocketCSharp.WebSockets
{
	class ClockWSEvents : WebSocketEvents, IDisposable
	{
		private readonly IWebSocketClient<ClockWSEvents> webSocketClient;

		private readonly Timer _timer;

		public ClockWSEvents(IWebSocketClient<ClockWSEvents> webSocketClient)
		{
			this.webSocketClient = webSocketClient;
			_timer = new Timer(TimerCallback, null, 1_000, 1_000);
		}

		public void Dispose() => _timer.Dispose();

		private void TimerCallback(object? state)
		{
			Task.Run(async () =>
			{
				await webSocketClient.SendToAll($"{DateTime.Now:HH:mm:ss} [{webSocketClient.Count} conectado(s)]", default);
			});
		}

		protected override async Task OnClientConnected(string clientId, CancellationToken cancellationToken)
		{
		}

		protected override async Task OnClientDisconnected(string clientId, CancellationToken cancellationToken)
		{
		}

		protected override async Task OnReceive(string message, string clientId, CancellationToken cancellationToken)
		{
		}
	}
}