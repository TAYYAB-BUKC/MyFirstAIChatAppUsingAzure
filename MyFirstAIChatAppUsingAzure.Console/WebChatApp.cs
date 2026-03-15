using HtmlAgilityPack;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;

namespace MyFirstAIChatAppUsingAzure.Console
{
	public partial class WebChatApp(HttpClient httpClient, IChatClient ai, IHostApplicationLifetime applicationLifetime) : BackgroundService
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
			systemMessage.Contents.Add(new TextContent("number_of_sentences=4"));
			history.Add(systemMessage);

			string data = await httpClient.GetStringAsync("https://dometrain.com/course/from-zero-to-hero-working-with-null-in-csharp/");
			// Parse HTML and extract text from <p> tags
			var doc = new HtmlDocument();
			doc.LoadHtml(data);

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
