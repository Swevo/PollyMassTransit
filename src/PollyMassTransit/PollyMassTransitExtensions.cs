/// <summary>Extension methods for adding Polly resilience to MassTransit bus instances.</summary>
public static class PollyMassTransitExtensions
{
    /// <summary>Wraps an <see cref="IBus"/> with the given <see cref="ResiliencePipeline"/>.</summary>
    public static ResilientBus WithPolly(
        this IBus bus,
        ResiliencePipeline pipeline)
        => new(bus, pipeline);

    /// <summary>Wraps an <see cref="IBus"/> with a pipeline built by <paramref name="configure"/>.</summary>
    public static ResilientBus WithPolly(
        this IBus bus,
        Action<ResiliencePipelineBuilder> configure)
    {
        var builder = new ResiliencePipelineBuilder();
        configure(builder);
        return new(bus, builder.Build());
    }
}
