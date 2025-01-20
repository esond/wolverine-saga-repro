using JasperFx.Core;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Orders.Api;

public record CreateOrderCommand(string ItemName);

public static class CreateOrderEndpoint
{
    [WolverinePost("/orders")]
    public static (IResult, OrderCreated) CreateOrder([FromBody] CreateOrderCommand command)
    {
        var orderId = CombGuidIdGeneration.NewGuid();

        var result = new Order
        {
            Id = orderId,
            ItemName = command.ItemName,
            Status = OrderStatus.Pending,
            Version = 0
        };

        return (TypedResults.Ok(result), new OrderCreated(orderId, command.ItemName));
    }
}
