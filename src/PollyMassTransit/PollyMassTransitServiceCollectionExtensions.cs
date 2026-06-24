/// <summary>Dependency-injection extensions for <c>PollyMassTransit</c>.</summary>
public static class PollyMassTransitServiceCollectionExtensions
{
    /// <summary>
    /// Registers a singleton <see cref="ResiliencePipeline"/> built by <paramref name="configure"/>
    /// and a transient <see cref="ResilientBus"/> that wraps the <see cref="IBus"/>
    /// already registered in the DI container (e.g., by <c>services.AddMassTransit(...)</c>).
    /// </summary>
    public static IServiceCollection AddPollyMassTransit(
        this IServiceCollection services,
        Action<ResiliencePipelineBuilder> configure)
    {
        var builder = new ResiliencePipelineBuilder();
        configure(builder);
        var pipeline = builder.Build();

        services.AddSingleton(pipeline);
        services.AddTransient<ResilientBus>(sp =>
            sp.GetRequiredService<IBus>().WithPolly(pipeline));

        return services;
    }
}
