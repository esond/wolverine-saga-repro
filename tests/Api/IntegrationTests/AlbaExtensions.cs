using System.Diagnostics.CodeAnalysis;
using System.Net;
using Alba;
using Wolverine.Tracking;

namespace Orders.Api.IntegrationTests;

public static class AlbaExtensions
{
    /// <summary>
    /// Ensures that any outgoing messages and cascaded work spawned by the initial request is completed before passing
    /// control back to the calling test.
    /// </summary>
    public static async Task<IScenarioResult> TrackedScenario(this IAlbaHost system, Action<Scenario> configuration,
        TimeSpan? timeout = null)
    {
        IScenarioResult result = null!;

        // We increase default timeout of 5 seconds to 30 seconds for easier debugging. This way we can sit on a
        // breakpoint for more than a moment without the whole operation timing out and failing.
        timeout ??= TimeSpan.FromSeconds(30);

        // Tie into Wolverine's test support to "wait" for all detected message activity to complete
        await system.ExecuteAndWaitAsync(async () =>
        {
            result = await system.Scenario(configuration);
        }, (int) timeout.Value.TotalMilliseconds);

        return result;
    }

    public static TrackingResponseExpression PostJsonTracked<T>(this IAlbaHost system, T request,
        [StringSyntax(StringSyntaxAttribute.Uri)] string url, JsonStyle? jsonStyle = null)
        where T : class
    {
        return new TrackingResponseExpression(system, s =>
        {
            s.WriteJson(request, jsonStyle);
            s.Post.Json(request, jsonStyle).ToUrl(url);

            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });
    }

    public class TrackingResponseExpression(IAlbaHost system, Action<Scenario> configure)
        : AlbaHostExtensions.ResponseExpression(system, configure)
    {
        private readonly IAlbaHost _mySystem = system;
        private readonly Action<Scenario> _myConfigure = configure;

        /// <summary>
        /// Ensures that any outgoing messages and cascaded work spawned by the initial request is completed before
        /// deserializing the response and passing control back to the test.
        /// </summary>
        public async Task<TResponse?> ReceiveTracked<TResponse>()
        {
            var response = await _mySystem.TrackedScenario(_myConfigure);

            return await response.ReadAsJsonAsync<TResponse>();
        }
    }
}
