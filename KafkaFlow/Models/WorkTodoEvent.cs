using System.ComponentModel;


namespace Models;

public class WorkTodoEvent
{
    public required string Description { get; set; }

    public string? Note { get; set; } 
    
    [DefaultValue("")]
    public string? Note2 { get; set; }
    
    [DefaultValue("")]
    public string? Note3 { get; set; } 
}