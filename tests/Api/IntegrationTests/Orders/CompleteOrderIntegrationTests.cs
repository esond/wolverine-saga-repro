using System.Net;
using Wolverine.Tracking;

namespace Orders.Api.IntegrationTests.Orders;

public class CompleteOrderIntegrationTests(WebApplicationFixture appFixture) : IntegrationTestBase(appFixture)
{
    [Fact]
    public async Task Expired_order_cannot_be_completed()
    {
        var createdOrder = await Host
            .PostJsonTracked(new CreateOrderCommand("foo"), "/orders")
            .ReceiveTracked<Order>();

        await Host.SendMessageAndWaitAsync(new OrderExpired(createdOrder!.Id, TimeSpan.Zero));

        await Host.Scenario(s =>
        {
            s.Post
                .Url($"/orders/{createdOrder.Id}/complete");

            s.StatusCodeShouldBe(HttpStatusCode.Conflict);
        });
    }
}
