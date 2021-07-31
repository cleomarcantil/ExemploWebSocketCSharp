using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelpersExtensions.WebSocketConnection
{
	class WebSocketConnectionManager : IWebSocketClient
	{
		private Dictionary<string, WebSocket> clients = new();

		public void Add(string clientId, WebSocket socket)
		{
			clients.Add(clientId, socket);
		}

		public void Remove(string clientId)
		{
			clients.Remove(clientId);
		}


		public async Task SendTo(string msg, string clientId)
		{
			var socket = clients.GetValueOrDefault(clientId);

			if (socket is null)
				return;

			if (socket.State != WebSocketState.Open)
				return;

			var data = Encoding.UTF8.GetBytes(msg);
			await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
		}

		public async Task SendTo(string msg, IEnumerable<string> clientIds)
		{
			var clients = this.clients.Keys.Intersect(clientIds);

			await Parallel.ForEachAsync(clients, async (clientId, cts) =>
			{
				await SendTo(msg, clientId);
			});
		}

		public async Task SendToAll(string msg)
		{
			var clients = this.clients.Keys;

			await Parallel.ForEachAsync(clients, async (clientId, cts) =>
			{
				await SendTo(msg, clientId);
			});
		}

		public async Task SendToAllExcept(string msg, IEnumerable<string> exceptClientIds)
		{
			var clients = this.clients.Keys.Except(exceptClientIds);

			await Parallel.ForEachAsync(clients, async (clientId, cts) =>
			{
				await SendTo(msg, clientId);
			});
		}


		public IEnumerable<string> GetClients() => clients.Keys;
		
		public int Count => clients.Count;

	}
}
