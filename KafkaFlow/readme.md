# Kafka Flow

## Description

This project is a simple example of how to use kafka flow to create work and training todos.

KafkaFlowDemo project is a MinimalApi which produces events

features:
- Json Serializer
- Multiple message (workTodo / trainingTodo) types in single topic

## How to run

Spin up the kafka docker containers through docker-compose

dotnet run the producer and consumer

## Avro

## Key Differences Between JSON Schema and Avro
When transitioning from JSON Schema to Avro, keep in mind these important differences:
1. **Schema Evolution**: Avro has more robust schema evolution capabilities
2. **Binary Format**: Avro serializes to a compact binary format (JSON Schema typically works with JSON)
3. **Schema Inclusion**: Avro doesn't include the schema with each message (relies on Schema Registry)
4. **Type System**: Avro has a different type system than JSON Schema
5. **Naming Conventions**: Avro has stricter naming rules for fields and types

## Handling C# Types and Mapping to Avro
Chr.Avro handles common C# types well, but here's how it maps types:
- `string` → Avro `string`
- `int`, `long`, etc. → Avro numeric types
- `bool` → Avro `boolean`
- `DateTime` → Avro `long` with logical type `timestamp-millis`
- `byte[]` → Avro `bytes`
- `enum` → Avro `enum`
- Classes → Avro `record`
- Collections → Avro `array`
- Dictionaries → Avro `map`
- Nullable types → Avro union with `null`

dependencies : 
- KafkaFlow nuget package

reference : 
- [kafka flow](https://farfetch.github.io/kafkaflow/docs/)
- [githup example](https://github.com/farfetch/kafkaflow)
- [youtube tutorial](https://www.youtube.com/watch?v=4e18DZkf-m0&t=644s&ab_channel=GuiFerreira)
- https://github.com/Farfetch/kafkaflow/tree/master/samples/KafkaFlow.Sample.SchemaRegistry
- https://medium.com/@nicolas-31/event-streaming-with-net-and-kafka-part-2-schemas-serialization-549b117cfae9