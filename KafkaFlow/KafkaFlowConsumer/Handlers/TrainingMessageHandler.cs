// using KafkaFlow;
// using Models;
//
// namespace KafkaFlowConsumer;
//
// public class TrainingMessageHandler //: IMessageHandler<TrainingTodoEvent>
// {
//     public Task Handle(IMessageContext context, TrainingTodoEvent message)
//     {
//         Console.WriteLine(
//             "Training Todo Event\n" +
//             "Partition: {0} | Offset: {1} | Message: {2}",
//             context.ConsumerContext.Partition,
//             context.ConsumerContext.Offset,
//             message.Description);
//
//         return Task.CompletedTask;
//     }
// }