using System;
using System.Collections.Generic;
using Avro;
using Avro.Specific;

namespace Models
{
    public class WorkTodoEvent : ISpecificRecord
    {
        public static Schema _SCHEMA = Schema.Parse(@"{
          ""type"": ""record"",
          ""name"": ""WorkTodoEvent"",
          ""namespace"": ""Models"",
          ""doc"": ""Event representing a work-related todo item"",
          ""fields"": [
            {
              ""name"": ""Description"",
              ""type"": ""string"",
              ""doc"": ""Description of the work todo item""
            }
          ]
        }");
        
        public string Description { get; set; }
        
        public Schema Schema => _SCHEMA;
        
        public object Get(int fieldPos)
        {
            switch (fieldPos)
            {
                case 0: return Description;
                default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
            }
        }
        
        public void Put(int fieldPos, object fieldValue)
        {
            switch (fieldPos)
            {
                case 0: Description = (string)fieldValue; break;
                default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
            }
        }
    }
}