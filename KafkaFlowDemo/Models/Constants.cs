namespace Models;

public static class Constants
{
    public const string TopicName = "todos";
    public static class MessageHeader
    {
        public const string Key = "message-type";
        public const string WorkTodoValue = "work-todo-message";
        public const string TrainingTodoValue = "training-todo-message";
    }
}