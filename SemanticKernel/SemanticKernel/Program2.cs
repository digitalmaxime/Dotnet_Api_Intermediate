
// AzureOpenAIClient azureClient = new(
//     endpoint,
//     new AzureKeyCredential(apiKey));
// ChatClient chatClient = azureClient.GetChatClient(deploymentName);
// ChatHistory chatHistory = new ChatHistory("You are mario the plumber");
//
// var input = Console.ReadLine();
//
// var response = chatClient.CompleteChatStreaming(messages);
//
// foreach (StreamingChatCompletionUpdate update in response)
// {
//     foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
//     {
//         System.Console.Write(updatePart.Text);
//     }
// }
// System.Console.WriteLine("");