using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpersExtensions.WebSocketConnection
{
	public interface IWebSocketClient
	{
		Task SendTo(string msg, string clientId);

		Task SendTo(string msg, IEnumerable<string> clientIds);

		Task SendToAll(string msg);

		Task SendToAllExcept(string msg, IEnumerable<string> exceptClientIds);

		IEnumerable<string> GetClients();

		int Count {  get; }
	}
}
