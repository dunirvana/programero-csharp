namespace OrderWorkflow.Before;

// Every action checks the current status by hand.
// Each new status means revisiting every method below.
public sealed class Order
{
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public int RefundCount { get; private set; }

    public void Pay()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot pay an order that is {Status}.");

        Status = OrderStatus.Paid;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException($"Cannot ship an order that is {Status}.");

        Status = OrderStatus.Shipped;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException($"Cannot deliver an order that is {Status}.");

        Status = OrderStatus.Delivered;
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new InvalidOperationException($"Cannot cancel an order that is {Status}.");

        if (Status != OrderStatus.Pending)
            RefundCount++;

        Status = OrderStatus.Cancelled;
    }

    // Added later, when returns became a requirement.
    // Nobody remembered to update Cancel().
    public void Return()
    {
        if (Status != OrderStatus.Delivered)
            throw new InvalidOperationException($"Cannot return an order that is {Status}.");

        RefundCount++;
        Status = OrderStatus.Returned;
    }
}
