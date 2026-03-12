using Microsoft.Extensions.Hosting;

namespace MyFirstAIChatAppUsingAzure_Console
{
	public class ChatApp(IHostApplicationLifetime applicationLifetime) : BackgroundService
	{
		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Console.WriteLine("You are an AI assistant that tries to answer the user's query.");
			applicationLifetime.StopApplication();
			return Task.CompletedTask;
		}
	}
}
