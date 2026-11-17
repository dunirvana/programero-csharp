namespace OrderWorkflow.After;

public sealed class PendingState : OrderState
{
    public override OrderStatus Status => OrderStatus.Pending;

    public override OrderState Pay(Order order) => new PaidState();
    public override OrderState Cancel(Order order) => new CancelledState();
}

public sealed class PaidState : OrderState
{
    public override OrderStatus Status => OrderStatus.Paid;

    public override OrderState Ship(Order order) => new ShippedState();

    public override OrderState Cancel(Order order)
    {
        order.IssueRefund();
        return new CancelledState();
    }
}

public sealed class ShippedState : OrderState
{
    public override OrderStatus Status => OrderStatus.Shipped;

    public override OrderState Deliver(Order order) => new DeliveredState();
}

public sealed class DeliveredState : OrderState
{
    public override OrderStatus Status => OrderStatus.Delivered;

    public override OrderState Return(Order order)
    {
        order.IssueRefund();
        return new ReturnedState();
    }
}

public sealed class CancelledState : OrderState
{
    public override OrderStatus Status => OrderStatus.Cancelled;
}

public sealed class ReturnedState : OrderState
{
    public override OrderStatus Status => OrderStatus.Returned;
}
