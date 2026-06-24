public class PollyMassTransitServiceCollectionExtensionsTests
{
    private static readonly IBus _bus = Substitute.For<IBus>();

    [Fact]
    public void AddPollyMassTransit_RegistersResiliencePipeline()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_bus);
        services.AddPollyMassTransit(p => { });
        Assert.NotNull(services.BuildServiceProvider().GetRequiredService<ResiliencePipeline>());
    }

    [Fact]
    public void AddPollyMassTransit_RegistersResilientBus()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_bus);
        services.AddPollyMassTransit(p => { });
        var resilient = services.BuildServiceProvider().GetRequiredService<ResilientBus>();
        Assert.NotNull(resilient);
        Assert.Same(_bus, resilient.Inner);
    }

    [Fact]
    public void AddPollyMassTransit_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_bus);
        Assert.Same(services, services.AddPollyMassTransit(p => { }));
    }
}
