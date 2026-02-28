namespace Application.Enums;

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