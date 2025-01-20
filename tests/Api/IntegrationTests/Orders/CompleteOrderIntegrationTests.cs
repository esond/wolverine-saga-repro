using System.Net;
using Alba;
using Wolverine.Tracking;

namespace Orders.Api.IntegrationTests.Orders;

public class CompleteOrderIntegrationTests(WebApplicationFixture appFixture) : IntegrationTestBase(appFixture)
{
    [Fact]
    public async Task Expired_order_cannot_be_completed()
    {
        var createdOrder = await Host
            .PostJson(new CreateOrderCommand("foo"), "/orders")
            .Receive<Order>();

        await Host.InvokeMessageAndWaitAsync(new OrderExpired(createdOrder!.Id, TimeSpan.Zero));

        await Host.Scenario(s =>
        {
            s.Post
                .Url($"/orders/{createdOrder.Id}/complete");

            s.StatusCodeShouldBe(HttpStatusCode.Conflict);
        });
    }
}
