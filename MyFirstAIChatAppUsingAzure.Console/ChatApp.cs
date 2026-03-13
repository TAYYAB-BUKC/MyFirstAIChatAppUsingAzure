using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;

namespace MyFirstAIChatAppUsingAzure_Console
{
	public class ChatApp(IHostApplicationLifetime applicationLifetime, IChatClient ai) : BackgroundService
	{
		private static bool exitRequested = false;

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Console.CancelKeyPress += (sender, e) =>
			{
				Console.WriteLine("\nCtrl+C detected. Exiting gracefully...");
				e.Cancel = true;
				applicationLifetime.StopApplication();
				exitRequested = true;
			};

			ChatMessage systemMessage = new(ChatRole.System, "You are an AI assistant that tries to answer the user's query.");
			ChatResponse response = await ai.GetResponseAsync(systemMessage);
			Console.WriteLine("AI: " + response.Text);
			
			while (stoppingToken.IsCancellationRequested == false)
			{
				Console.Write("Prompt > ");
				string? userMessage = Console.ReadLine();
				if (userMessage == null || exitRequested)
					break;

				var userRequest = new ChatMessage(ChatRole.User, userMessage);
				ChatResponse chatResponse = await ai.GetResponseAsync(userRequest);
				foreach (var msg in chatResponse.Messages)
				{
					Console.WriteLine($"{msg.Role}: {msg.Text}");
				}
			}
		}
	}
}
