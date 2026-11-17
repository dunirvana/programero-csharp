namespace OrderWorkflow.After;

// The order delegates every action to its current state.
public sealed class Order
{
    private OrderState _state = new PendingState();

    public OrderStatus Status => _state.Status;
    public int RefundCount { get; private set; }

    public void Pay() => _state = _state.Pay(this);
    public void Ship() => _state = _state.Ship(this);
    public void Deliver() => _state = _state.Deliver(this);
    public void Cancel() => _state = _state.Cancel(this);
    public void Return() => _state = _state.Return(this);

    internal void IssueRefund() => RefundCount++;
}
