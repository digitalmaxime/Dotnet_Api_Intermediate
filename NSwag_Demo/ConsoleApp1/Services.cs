// using Microsoft.Extensions.DependencyInjection;
// using MyNamespace;
// using MyClient = AutoGenApiClient.MyClient;
//
// namespace ConsoleApp1;
//
// public static class Services
// {
//     private static ServiceProvider? _serviceProvider;
//
//     public static ServiceProvider ServiceProvider
//     {
//         get
//         {
//             if (_serviceProvider == null)
//             {
//                 _serviceProvider = CreateServices();
//             }
//
//             return _serviceProvider;
//         }
//     }
//
//     private static ServiceProvider CreateServices()
//     {
//         var serviceCollection = new ServiceCollection();
//         //     
//         // serviceCollection
//             
//         serviceCollection
//             .AddSingleton<IMyApp, MyApp>();
//
//         return serviceCollection.BuildServiceProvider();
//     }
// }