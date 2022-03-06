using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelpersExtensions.WebSocketConnection
{
	class WebSocketClient<TWSEvents> : IWebSocketClient<TWSEvents>
		where TWSEvents : WebSocketEvents
	{
		private readonly IServiceProvider serviceProvider;

		public WebSocketClient(IServiceProvider serviceProvider) =>
			this.serviceProvider = serviceProvider;

		internal WebSocketConnections Connections { get; } = new();

		public async Task SendTo(string message, string clientId, CancellationToken cancellationToken)
		{
			var socket = Connections.GetWebSocket(clientId);

			if (socket is null || socket.State != WebSocketState.Open)
				return;

			var data = Encoding.UTF8.GetBytes(message);
			await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, cancellationToken);
		}

		public async Task SendTo(string message, IEnumerable<string> clientIds, CancellationToken cancellationToken)
		{
			clientIds = this.Connections.GetClients().Intersect(clientIds);

			await Parallel.ForEachAsync(clientIds, async (clientId, cts) =>
			{
				await SendTo(message, clientId, cancellationToken);
			});
		}

		public async Task SendToAll(string message, CancellationToken cancellationToken)
		{
			var clientIds = this.Connections.GetClients();

			await Parallel.ForEachAsync(clientIds, async (clientId, cts) =>
			{
				await SendTo(message, clientId, cancellationToken);
			});
		}

		public async Task SendToAllExcept(string message, IEnumerable<string> exceptClientIds, CancellationToken cancellationToken)
		{
			var clientIds = this.Connections.GetClients().Except(exceptClientIds);

			await Parallel.ForEachAsync(clientIds, async (clientId, cts) =>
			{
				await SendTo(message, clientId, cancellationToken);
			});
		}

		public IEnumerable<string> GetClients() =>
			Connections.GetClients();

		public int Count =>
			Connections.Count;

		public TWSEvents GetWSEvents() =>
			serviceProvider.GetRequiredService<TWSEvents>();

	}
}
