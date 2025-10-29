using Azure;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Configuration;

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.private.json").Build();

// 2. Get configuration values
var endpoint = new Uri(configuration["AzureOpenAI:Endpoint"]!);
var deploymentName = configuration["AzureOpenAI:DeploymentName"]!;
var apiKey = configuration["AzureOpenAI:ApiKey"]!;

AzureOpenAIClient azureClient = new(endpoint, new AzureKeyCredential(apiKey));

IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddAzureOpenAIChatCompletion(deploymentName, azureClient);

var kernel = kernelBuilder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();

// Persona
ChatHistory chatHistory =
    new ChatHistory(
        "You are mario the plumber! If the user doesn't provider a news catagory, assume they want technology news and mention it to them. Keep your responses short and concise. Always greet the user with a friendly 'It's-a me, Mario!'");

while (true)
{
    Console.Write("> ", ConsoleColor.Yellow);
    var userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput)) break;
    chatHistory.AddUserMessage(userInput);
    var response = chatService.GetStreamingChatMessageContentsAsync(
        chatHistory,
        executionSettings: new OpenAIPromptExecutionSettings()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        },
        kernel: kernel
    );
    var fullAnswer = "";
    await foreach (var chunk in response)
    {
        if (chunk.Content?.Length > 0)
        {
            fullAnswer += chunk.Content;
            Console.Write(chunk);
        }
    }

    Console.WriteLine(fullAnswer);
    chatHistory.AddAssistantMessage(fullAnswer);
    Console.WriteLine("\n");
}