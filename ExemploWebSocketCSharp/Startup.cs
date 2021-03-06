using ExemploWebSocketCSharp.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace ExemploWebSocketCSharp
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExemploWebSocketCSharp", Version = "v1" });
			});

			services.AddRazorPages();

			services.AddWebSocketEventsService<ClockWSEvents>();
			services.AddWebSocketEventsService<ChatWSEvents>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExemploWebSocketCSharp v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				//endpoints.MapControllers();
				endpoints.MapRazorPages();
			});

			app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new[] { "index.html" } });
			app.UseStaticFiles();

			app.UseWebSocketWithEventsCallback<ClockWSEvents>("/ws-clock", new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(120) });
			app.UseWebSocketWithEventsCallback<ChatWSEvents>("/ws-chat", new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(120) });

		}
		
	}
}
