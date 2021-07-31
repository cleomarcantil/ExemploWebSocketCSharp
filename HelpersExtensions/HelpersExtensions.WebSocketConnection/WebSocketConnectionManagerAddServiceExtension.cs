using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HelpersExtensions.WebSocketConnection
{
	public static class WebSocketConnectionManagerAddServiceExtension
	{
		public static IServiceCollection AddWebSocketConnectionManager<TWebSocketServerEvents>(this IServiceCollection services)
			where TWebSocketServerEvents : class, IWebSocketServerEvents
		{
			services.AddSingleton<IWebSocketClient, WebSocketConnectionManager>();
			services.AddSingleton<IWebSocketServerEvents, TWebSocketServerEvents>();

			return services;
		}

		public static IApplicationBuilder UseWebSocketWithConnectionManager(this IApplicationBuilder app, string path, WebSocketOptions options)
		{
			app.UseWebSockets(options);

			return app.Use(async (context, next) =>
			{
				if (context.Request.Path == path)
				{
					if (context.WebSockets.IsWebSocketRequest)
					{
						using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
						await WebSocketEcho.Invoke(context, webSocket);
					}
					else
					{
						context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
					}
				}
				else
				{
					await next();
				}
			});
		}
	}
}
