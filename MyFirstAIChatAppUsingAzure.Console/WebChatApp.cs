using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;

namespace MyFirstAIChatAppUsingAzure.Console
{
	public partial class WebChatApp(IHostApplicationLifetime applicationLifetime, IChatClient ai) : BackgroundService
	{
		private static bool exitRequested = false;
		List<ChatMessage> history = [];
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			System.Console.CancelKeyPress += (sender, e) =>
			{
				System.Console.WriteLine("\nCtrl+C detected. Exiting gracefully...");
				e.Cancel = true;
				applicationLifetime.StopApplication();
				exitRequested = true;
			};

			ChatMessage systemMessage = new(ChatRole.System, summarizationPrompt);
			history.Add(systemMessage);
			ChatResponse response = await ai.GetResponseAsync(history);
			System.Console.WriteLine("AI: " + response.Text);

			while (stoppingToken.IsCancellationRequested == false)
			{
				System.Console.Write("Prompt > ");
				string? userMessage = System.Console.ReadLine();
				if (userMessage == null || exitRequested)
					break;
				history.Add(new ChatMessage(ChatRole.User, userMessage));
				ChatResponse chatResponse = await ai.GetResponseAsync(history);
				history.AddMessages(chatResponse);
				foreach (var msg in chatResponse.Messages)
				{
					System.Console.WriteLine($"{msg.Role}: {msg.Text}");
				}
			}
		}
	}
}
