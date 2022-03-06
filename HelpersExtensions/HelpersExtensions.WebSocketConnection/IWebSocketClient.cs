using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HelpersExtensions.WebSocketConnection
{
	public interface IWebSocketClient<TWSEvents>
		where TWSEvents : WebSocketEvents
	{
		Task SendTo(string message, string clientId, CancellationToken cancellationToken);
		
		Task SendTo(string message, IEnumerable<string> clientIds, CancellationToken cancellationToken);

		Task SendToAll(string message, CancellationToken cancellationToken);

		Task SendToAllExcept(string message, IEnumerable<string> exceptClientIds, CancellationToken cancellationToken);

		IEnumerable<string> GetClients();

		int Count {  get; }

		TWSEvents GetWSEvents();
	}
}
