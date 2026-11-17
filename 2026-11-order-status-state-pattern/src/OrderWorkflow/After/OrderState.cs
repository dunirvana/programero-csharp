namespace OrderWorkflow.After;

// Base state: every action is forbidden unless a state allows it.
public abstract class OrderState
{
    public abstract OrderStatus Status { get; }

    public virtual OrderState Pay(Order order) => throw NotAllowed("pay");
    public virtual OrderState Ship(Order order) => throw NotAllowed("ship");
    public virtual OrderState Deliver(Order order) => throw NotAllowed("deliver");
    public virtual OrderState Cancel(Order order) => throw NotAllowed("cancel");
    public virtual OrderState Return(Order order) => throw NotAllowed("return");

    private InvalidOperationException NotAllowed(string action) =>
        new($"Cannot {action} an order that is {Status}.");
}
