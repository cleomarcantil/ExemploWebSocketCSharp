using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

namespace ExemploWebSocketCSharp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WebSocketController : ControllerBase
	{

		[HttpGet("/ws")]
		public async Task Get()
		{
			if (HttpContext.WebSockets.IsWebSocketRequest)
			{
				using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
				await Echo(HttpContext, webSocket);
			}
			else
			{
				HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			}
		}


		private async Task Echo(HttpContext context, WebSocket webSocket)
		{
			var buffer = new byte[1024 * 4];
			WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

			while (!result.CloseStatus.HasValue)
			{
				var mensagemRecebida = Encoding.UTF8.GetString(buffer);
				Console.WriteLine($"Mensagem recebida: {mensagemRecebida}");

				var mensagemParaEnviar = Encoding.UTF8.GetBytes($"Do servidor: {DateTime.Now}");
				await webSocket.SendAsync(new ArraySegment<byte>(mensagemParaEnviar), result.MessageType, result.EndOfMessage, CancellationToken.None);

				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}

			await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
		}

	}
}
