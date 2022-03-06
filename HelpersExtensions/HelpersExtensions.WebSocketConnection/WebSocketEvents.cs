using System.Threading;
using System.Threading.Tasks;

namespace HelpersExtensions.WebSocketConnection
{
	public abstract class WebSocketEvents
	{
		protected internal abstract Task OnClientConnected(string clientId, CancellationToken cancellationToken);

		protected internal abstract Task OnReceive(string message, string clientId, CancellationToken cancellationToken);

		protected internal abstract Task OnClientDisconnected(string clientId, CancellationToken cancellationToken);
	}
}
