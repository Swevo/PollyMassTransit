public class PollyMassTransitExtensionsTests
{
    private static readonly IBus _bus = Substitute.For<IBus>();
    private static readonly ResiliencePipeline _pipeline = new ResiliencePipelineBuilder().Build();

    [Fact]
    public void WithPolly_Pipeline_ReturnsResilientBus()
    {
        var resilient = _bus.WithPolly(_pipeline);
        Assert.NotNull(resilient);
        Assert.Same(_bus, resilient.Inner);
    }

    [Fact]
    public void WithPolly_Configure_ReturnsResilientBus()
    {
        var resilient = _bus.WithPolly(p => p.AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3, Delay = TimeSpan.Zero,
            ShouldHandle = MassTransitTransientErrors.IsTransient,
        }));
        Assert.NotNull(resilient);
        Assert.Same(_bus, resilient.Inner);
    }
}
