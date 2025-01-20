using Wolverine.Http;
using Wolverine.Http.Marten;

namespace Orders.Api;

public class CompleteOrderEndpoint
{
    [WolverinePost("/orders/{id}/complete")]
    public static (IResult, OrderCompleted?) CompleteOrder([Document(Required = true)] Order order)
    {
        if (order.Status == OrderStatus.Expired)
            return (TypedResults.Conflict("Order is expired"), null);

        order.Status = OrderStatus.Completed;

        return (TypedResults.Ok(order), new OrderCompleted(order.Id));
    }
}
