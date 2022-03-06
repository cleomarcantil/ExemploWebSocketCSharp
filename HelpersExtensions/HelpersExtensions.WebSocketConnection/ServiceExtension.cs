using HelpersExtensions.WebSocketConnection;
using Microsoft.AspNetCore.Builder;
using System;
using System.IO;
using System.Net;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceExtension
	{
		public static IServiceCollection AddWebSocketEventsService<TWSEvents>(this IServiceCollection services)
			where TWSEvents : WebSocketEvents
		{
			services.AddSingleton<IWebSocketClient<TWSEvents>, WebSocketClient<TWSEvents>>();
			services.AddSingleton<TWSEvents>();

			return services;
		}

		public static IApplicationBuilder UseWebSocketWithEventsCallback<TWSEvents>(this IApplicationBuilder app, string path, WebSocketOptions options, bool allowAnonymous = true)
			where TWSEvents : WebSocketEvents
		{
			app.UseWebSockets(options);

			return app.Use(async (context, next) =>
			{
				if (context.Request.Path == path)
				{
					if (!allowAnonymous && context.User.Identity?.IsAuthenticated is not true)
						context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
					else if (context.WebSockets.IsWebSocketRequest)
					{
						using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
						try
						{
							await WebSocketEcho<TWSEvents>.Invoke(context, webSocket, context.RequestAborted);
						}
						catch (OperationCanceledException ex)
						{
						}
						catch (Exception ex)
						{
							context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
							// TODO: Tratar erro 
						}
					}
					else
						context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				}
				else
					await next();
			});
		}
	}
}
