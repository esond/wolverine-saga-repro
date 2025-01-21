using Wolverine;

namespace Orders.Api;

public record OrderCreated(Guid Id, string ItemName);

public record OrderCompleted(Guid Id);

public record OrderExpired(Guid Id, TimeSpan DelayTime) : TimeoutMessage(DelayTime);

public enum OrderStatus
{
    Pending,
    Completed,
    Expired
}

public class Order : Saga
{
    //public Guid Id { get; set; }

    //public string ItemName { get; set; } = null!;

    //public OrderStatus Status { get; set; }

    public required Guid Id { get; set; }

    public required string ItemName { get; set; }

    public required OrderStatus Status { get; set; }

    public static (Order, OrderExpired) Start(OrderCreated created)
    {
        var order = new Order
        {
            Id = created.Id,
            ItemName = created.ItemName,
            Status = OrderStatus.Pending
        };

        return (order, new OrderExpired(order.Id, TimeSpan.FromSeconds(15)));
    }

    public void Handle(OrderCompleted completed, ILogger<Order> logger)
    {
        Status = OrderStatus.Completed;

        logger.LogInformation("Order {OrderId} completed", Id);
    }

    public void Handle(OrderExpired expired)
    {
        Status = OrderStatus.Expired;
    }

    public void NotFound(OrderExpired expired, ILogger<Order> logger)
    {
        logger.LogWarning("Tried to expire order {OrderId}, but it was not found", expired.Id);
    }
}
