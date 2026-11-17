using OrderWorkflow.Before;
using Xunit;

namespace OrderWorkflow.Tests;

public class BeforeOrderTests
{
    [Fact]
    public void Happy_path_reaches_delivered()
    {
        var order = new Order();

        order.Pay();
        order.Ship();
        order.Deliver();

        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    // Documents the limitation: Return() was added, Cancel() was not revisited,
    // so a returned order can be cancelled and refunded twice.
    [Fact]
    public void Cancelling_a_returned_order_refunds_twice()
    {
        var order = new Order();
        order.Pay();
        order.Ship();
        order.Deliver();
        order.Return();

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Equal(2, order.RefundCount);
    }
}
