public class MassTransitTransientErrorsTests
{
    [Fact]
    public void IsTransient_IsNotNull()
        => Assert.NotNull(MassTransitTransientErrors.IsTransient);

    [Fact]
    public async Task IsTransient_RetriesRequestTimeoutException()
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 1, Delay = TimeSpan.Zero, ShouldHandle = MassTransitTransientErrors.IsTransient })
            .Build();

        var attempts = 0;
        await Assert.ThrowsAsync<RequestTimeoutException>(() =>
            pipeline.ExecuteAsync(ct => { attempts++; throw new RequestTimeoutException(); }).AsTask());

        Assert.Equal(2, attempts);
    }

    [Fact]
    public async Task IsTransient_RetriesTimeoutException()
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 1, Delay = TimeSpan.Zero, ShouldHandle = MassTransitTransientErrors.IsTransient })
            .Build();

        var attempts = 0;
        await Assert.ThrowsAsync<TimeoutException>(() =>
            pipeline.ExecuteAsync(ct => { attempts++; throw new TimeoutException(); }).AsTask());

        Assert.Equal(2, attempts);
    }

    [Fact]
    public async Task IsTransient_RetriesHttpRequestException()
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 1, Delay = TimeSpan.Zero, ShouldHandle = MassTransitTransientErrors.IsTransient })
            .Build();

        var attempts = 0;
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            pipeline.ExecuteAsync(ct => { attempts++; throw new HttpRequestException("network"); }).AsTask());

        Assert.Equal(2, attempts);
    }

    [Fact]
    public async Task IsTransient_RetriesTaskCanceledException()
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 1, Delay = TimeSpan.Zero, ShouldHandle = MassTransitTransientErrors.IsTransient })
            .Build();

        var attempts = 0;
        await Assert.ThrowsAsync<TaskCanceledException>(() =>
            pipeline.ExecuteAsync(ct => { attempts++; throw new TaskCanceledException(); }).AsTask());

        Assert.Equal(2, attempts);
    }
}
