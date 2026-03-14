using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyFirstAIChatAppUsingAzure.Console;
using MyFirstAIChatAppUsingAzure_Console;

var host = Host.CreateApplicationBuilder(args);

host.Configuration.AddUserSecrets<Program>();

var endpoint = host.Configuration["Chat:AI:Endpoint"] ?? throw new InvalidOperationException("Missing configuration: Endpoint. See the README for details");
var apiKey = host.Configuration["Chat:AI:ApiKey"] ?? throw new InvalidOperationException("Missing configuration: ApiKey. See the README for details");

var client = new AzureOpenAIClient(
	new Uri(endpoint),
	new AzureKeyCredential(apiKey)
);

string model = "gpt-4o-mini";
IChatClient innerClient = client.GetChatClient(model).AsIChatClient();

host.Services.AddChatClient(innerClient);
host.Services.AddHttpClient();
//host.Services.AddHostedService<ChatApp>();
host.Services.AddHostedService<WebChatApp>();

var app = host.Build();
await app.RunAsync();