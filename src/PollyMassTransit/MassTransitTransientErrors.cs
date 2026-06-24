/// <summary>
/// Pre-built Polly <see cref="PredicateBuilder"/> for transient MassTransit producer errors.
/// Covers request timeouts, general timeouts, and transient network failures.
/// </summary>
public static class MassTransitTransientErrors
{
    /// <summary>
    /// A <see cref="PredicateBuilder"/> that handles:
    /// <list type="bullet">
    ///   <item><see cref="RequestTimeoutException"/> — request-reply timed out waiting for response</item>
    ///   <item><see cref="TimeoutException"/> — operation timed out at the transport layer</item>
    ///   <item><see cref="HttpRequestException"/> — HTTP transport network failure</item>
    ///   <item><see cref="TaskCanceledException"/> — operation cancelled due to timeout or network failure</item>
    /// </list>
    /// Assign to <c>ShouldHandle</c> on any Polly strategy.
    /// </summary>
    public static readonly PredicateBuilder IsTransient =
        (PredicateBuilder)new PredicateBuilder()
            .Handle<RequestTimeoutException>()
            .Handle<TimeoutException>()
            .Handle<HttpRequestException>()
            .Handle<TaskCanceledException>();
}
