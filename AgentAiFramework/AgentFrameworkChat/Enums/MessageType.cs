namespace AgentFrameworkChat.Enums;

public enum MessageType
{
    Text,
    Reasoning,
    FunctionCall,
    FunctionResult,
    FunctionApprovalRequest,
    FunctionApprovalResponse,
    Error
}