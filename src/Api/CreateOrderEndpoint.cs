using JasperFx.Core;
using Wolverine.Http;

namespace Orders.Api;

public record CreateOrderCommand(string ItemName);

public static class CreateOrderEndpoint
{
    [WolverinePost("/orders")]
    public static (Order, OrderCreated) CreateOrder(CreateOrderCommand command)
    {
        var orderId = CombGuidIdGeneration.NewGuid();

        var createdOrder = new Order
        {
            Id = orderId,
            ItemName = command.ItemName,
            Status = OrderStatus.Pending,
            Version = 0
        };

        return (createdOrder, new OrderCreated(orderId, command.ItemName));
    }
}
