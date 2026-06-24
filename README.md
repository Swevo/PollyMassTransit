# PollyMassTransit

[![NuGet](https://img.shields.io/nuget/v/PollyMassTransit.svg)](https://www.nuget.org/packages/PollyMassTransit/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/PollyMassTransit.svg)](https://www.nuget.org/packages/PollyMassTransit/)
[![CI](https://github.com/Swevo/PollyMassTransit/actions/workflows/build.yml/badge.svg)](https://github.com/Swevo/PollyMassTransit/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Polly v8 producer-side resilience for MassTransit** — add retry, timeout, and circuit-breaker to `IBus.Publish` and `SendAsync` in two lines.

> **Key differentiator:** MassTransit's built-in retry only applies to *consumers*. This library fills the producer-side gap.

```csharp
var resilient = bus.WithPolly(pipeline => pipeline
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        ShouldHandle = MassTransitTransientErrors.IsTransient,
    })
    .AddTimeout(TimeSpan.FromSeconds(10)));

await resilient.PublishAsync(new OrderPlaced { OrderId = id }, cancellationToken);
```

## Why PollyMassTransit?

MassTransit's retry middleware sits on the *consumer* pipeline — it retries message processing. When the *producer* (your application code publishing or sending messages) fails due to a transient broker outage, MassTransit does nothing. This library adds Polly v8 to the producer side:

| Problem | Solution |
|---------|----------|
| `RequestTimeoutException` during request-reply | Caught by `MassTransitTransientErrors.IsTransient` |
| `TimeoutException` at the transport layer | Caught by `MassTransitTransientErrors.IsTransient` |
| `HttpRequestException` (HTTP transport) | Caught by `MassTransitTransientErrors.IsTransient` |
| `TaskCanceledException` timeout in transit | Caught by `MassTransitTransientErrors.IsTransient` |
| Cascading failures during broker downtime | Wrap with `AddCircuitBreaker` |

## Installation

```
dotnet add package PollyMassTransit
dotnet add package Polly.Core
```

## Quick-start

### 1. Manual wiring

```csharp
// bus is the IBus registered by services.AddMassTransit(...)
var resilient = bus.WithPolly(p => p
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        ShouldHandle = MassTransitTransientErrors.IsTransient,
    }));

await resilient.PublishAsync(new OrderPlaced { OrderId = orderId }, ct);
await resilient.SendAsync(new Uri("queue:orders"), new CreateOrder { ... }, ct);
```

### 2. Dependency injection

```csharp
// After services.AddMassTransit(...)
builder.Services.AddPollyMassTransit(pipeline => pipeline
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        ShouldHandle = MassTransitTransientErrors.IsTransient,
    })
    .AddTimeout(TimeSpan.FromSeconds(10)));

public class OrderService(ResilientBus bus)
{
    public Task PlaceOrderAsync(Order order, CancellationToken ct)
        => bus.PublishAsync(new OrderPlaced { OrderId = order.Id }, ct);
}
```

## API reference

| Member | Description |
|--------|-------------|
| `ResilientBus.Inner` | The underlying `IBus` |
| `PublishAsync<T>(message, ct)` | Publishes a message through the pipeline |
| `PublishAsync<T>(message, pipe, ct)` | Publishes with a pipe configuration through the pipeline |
| `SendAsync<T>(address, message, ct)` | Sends to an endpoint through the pipeline |
| `ExecuteAsync<T>(operation, ct)` | Runs any `IBus` operation through the pipeline |
| `MassTransitTransientErrors.IsTransient` | `PredicateBuilder` for `RequestTimeoutException`, `TimeoutException`, `HttpRequestException`, `TaskCanceledException` |
| `bus.WithPolly(pipeline)` | Wraps `IBus` with a pre-built pipeline |
| `bus.WithPolly(configure)` | Builds pipeline inline and wraps the bus |
| `services.AddPollyMassTransit(configure)` | DI registration (requires `IBus` in DI) |

## Target frameworks

.NET 6 ✅ · .NET 8 ✅ · .NET 9 ✅

## Related packages

| Package | Description |
|---------|-------------|
| [PollyRabbitMQ](https://github.com/Swevo/PollyRabbitMQ) | Polly v8 for RabbitMQ |
| [PollyKafka](https://github.com/Swevo/PollyKafka) | Polly v8 for Confluent.Kafka |
| [PollyAzureServiceBus](https://github.com/Swevo/PollyAzureServiceBus) | Polly v8 for Azure Service Bus |
| [PollyAzureEventHub](https://github.com/Swevo/PollyAzureEventHub) | Polly v8 for Azure Event Hubs |
| [PollySignalR](https://github.com/Swevo/PollySignalR) | Polly v8 for SignalR |
| [PollyGrpc](https://github.com/Swevo/PollyGrpc) | Polly v8 for gRPC |
| [PollyRedis](https://github.com/Swevo/PollyRedis) | Polly v8 for StackExchange.Redis |
| [PollyEFCore](https://github.com/Swevo/PollyEFCore) | Polly v8 for Entity Framework Core |
| [PollyDapper](https://github.com/Swevo/PollyDapper) | Polly v8 for Dapper |
| [PollyMongo](https://github.com/Swevo/PollyMongo) | Polly v8 for MongoDB |
| [PollyNpgsql](https://github.com/Swevo/PollyNpgsql) | Polly v8 for Npgsql (PostgreSQL) |
| [PollySqlClient](https://github.com/Swevo/PollySqlClient) | Polly v8 for Microsoft.Data.SqlClient |
| [PollyCosmosDb](https://github.com/Swevo/PollyCosmosDb) | Polly v8 for Azure Cosmos DB |
| [PollyAzureBlob](https://github.com/Swevo/PollyAzureBlob) | Polly v8 for Azure Blob Storage |
| [PollyOpenAI](https://github.com/Swevo/PollyOpenAI) | Polly v8 for OpenAI .NET SDK |
| [PollyMediatR](https://github.com/Swevo/PollyMediatR) | Polly v8 for MediatR |
| [PollyHealthChecks](https://github.com/Swevo/PollyHealthChecks) | Polly v8 for ASP.NET Core Health Checks |
| [PollySendGrid](https://github.com/Swevo/PollySendGrid) | Polly v8 for SendGrid |
| [PollyAzureTableStorage](https://github.com/Swevo/PollyAzureTableStorage) | Polly v8 for Azure Table Storage |
| [PollyMailKit](https://github.com/Swevo/PollyMailKit) | MailKit SMTP email client |
| [PollyAzureQueueStorage](https://github.com/Swevo/PollyAzureQueueStorage) | Azure Queue Storage QueueClient |
| [PollyHangfire](https://github.com/Swevo/PollyHangfire) | Hangfire IBackgroundJobClient |
| [PollyBackoff](https://github.com/Swevo/PollyBackoff) | Polly v8 backoff helpers |

## License

MIT © [Justin Bannister](https://github.com/Swevo)