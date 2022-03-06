using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

namespace HelpersExtensions.WebSocketConnection
{
	internal class WebSocketConnections
	{
		private Dictionary<string, WebSocket> clients = new();

		internal void Add(string clientId, WebSocket socket) =>
			clients.Add(clientId, socket);

		internal void Remove(string clientId) =>
			clients.Remove(clientId);

		public WebSocket? GetWebSocket(string clientId) =>
			clients.GetValueOrDefault(clientId);

		public IEnumerable<string> GetClients() =>
			clients.Keys;

		public int Count =>
			clients.Count;
	}
}
