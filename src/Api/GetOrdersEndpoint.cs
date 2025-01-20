using Marten;
using Wolverine.Http;

namespace Orders.Api;

public class GetOrdersEndpoint
{
    [WolverineGet("/orders")]
    public static async Task<IResult> GetOrders(IQuerySession session)
    {
        var orders = await session.Query<Order>().ToListAsync();

        return TypedResults.Ok(orders);
    }
}
