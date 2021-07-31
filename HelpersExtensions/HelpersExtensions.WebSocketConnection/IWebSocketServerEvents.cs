using System.Threading.Tasks;

namespace HelpersExtensions.WebSocketConnection
{
	public interface IWebSocketServerEvents
	{
		Task OnClientConnected(string clientId);

		Task OnReceiveMessage(string msg, string clientId);

		Task OnClientDisconnected(string clientId);

	}
}
