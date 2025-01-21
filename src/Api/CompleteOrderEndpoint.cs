using Microsoft.AspNetCore.Http.HttpResults;
using Wolverine.Http;
using Wolverine.Http.Marten;

namespace Orders.Api;

public class CompleteOrderEndpoint
{
    public static Conflict<string>? Validate(Order order)
    {
        return order.Status == OrderStatus.Expired ? TypedResults.Conflict("Order is expired") : null;
    }

    [WolverinePost("/orders/{id}/complete")]
    public static (Order, OrderCompleted) CompleteOrder([Document(Required = true)] Order order)
    {
        order.Status = OrderStatus.Completed;

        return (order, new OrderCompleted(order.Id));
    }
}
