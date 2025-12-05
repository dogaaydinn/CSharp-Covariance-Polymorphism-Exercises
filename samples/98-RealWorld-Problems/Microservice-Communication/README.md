# Microservice Communication

## Problem
10 microservices communicating synchronously. One service down = entire system fails. Latency issues.

## Solutions
1. **Basic**: REST API + Retry logic
2. **Advanced**: Message Queue (RabbitMQ) for async communication
3. **Enterprise**: Event-Driven Architecture + CQRS + Saga pattern

## Communication Patterns
- **Synchronous**: REST, gRPC (request/response)
- **Asynchronous**: Message queues, event bus
- **Hybrid**: Sync for queries, async for commands

## Challenges
- Service discovery
- Load balancing
- Circuit breaker (prevent cascading failures)
- Distributed tracing
- Data consistency (eventual consistency)

## Patterns
- API Gateway (single entry point)
- Service Mesh (Istio, Linkerd)
- Saga Pattern (distributed transactions)
- CQRS (Command Query Responsibility Segregation)
- Event Sourcing

See PROBLEM.md for implementation examples with MassTransit/NServiceBus.
