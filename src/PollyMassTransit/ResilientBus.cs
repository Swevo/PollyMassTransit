/// <summary>
/// Wraps an <see cref="IBus"/> with a Polly v8 <see cref="ResiliencePipeline"/>,
/// applying retry, timeout, and circuit-breaker to every publish and send operation.
/// Addresses the producer-side resilience gap — MassTransit's built-in retry
/// only applies to consumers, not to publish or send calls.
/// </summary>
public sealed class ResilientBus(IBus bus, ResiliencePipeline pipeline)
{
    /// <summary>The underlying <see cref="IBus"/>.</summary>
    public IBus Inner => bus;

    /// <summary>Publishes a message, protected by the resilience pipeline.</summary>
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class
        => pipeline.ExecuteAsync(
            async ct => { await bus.Publish(message, ct); return 0; },
            cancellationToken).AsTask();

    /// <summary>Publishes a message with a pipe configuration, protected by the resilience pipeline.</summary>
    public Task PublishAsync<T>(
        T message,
        IPipe<PublishContext<T>> pipe,
        CancellationToken cancellationToken = default)
        where T : class
        => pipeline.ExecuteAsync(
            async ct => { await bus.Publish(message, pipe, ct); return 0; },
            cancellationToken).AsTask();

    /// <summary>
    /// Sends a message to a specific endpoint address, protected by the resilience pipeline.
    /// </summary>
    public Task SendAsync<T>(
        Uri destinationAddress,
        T message,
        CancellationToken cancellationToken = default)
        where T : class
        => pipeline.ExecuteAsync(async ct =>
        {
            var endpoint = await bus.GetSendEndpoint(destinationAddress);
            await endpoint.Send(message, ct);
            return 0;
        }, cancellationToken).AsTask();

    /// <summary>
    /// Executes any <see cref="IBus"/> operation, protected by the resilience pipeline.
    /// </summary>
    public Task<T> ExecuteAsync<T>(
        Func<IBus, CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
        => pipeline.ExecuteAsync(
            async ct => await operation(bus, ct),
            cancellationToken).AsTask();
}
