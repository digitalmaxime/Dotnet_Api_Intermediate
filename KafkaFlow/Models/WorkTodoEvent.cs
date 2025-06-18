using System;
using System.Collections.Generic;
using Avro;
using Avro.Specific;

namespace Models;

public class WorkTodoEvent
{
    public required string Description { get; set; }
}