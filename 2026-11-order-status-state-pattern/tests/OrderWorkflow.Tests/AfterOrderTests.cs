using OrderWorkflow.After;
using Xunit;

namespace OrderWorkflow.Tests;

public class AfterOrderTests
{
    private static Order DeliveredOrder()
    {
        var order = new Order();
        order.Pay();
        order.Ship();
        order.Deliver();
        return order;
    }

    [Fact]
    public void New_order_starts_pending()
    {
        Assert.Equal(OrderStatus.Pending, new Order().Status);
    }

    [Fact]
    public void Happy_path_reaches_delivered()
    {
        var order = DeliveredOrder();

        Assert.Equal(OrderStatus.Delivered, order.Status);
        Assert.Equal(0, order.RefundCount);
    }

    [Fact]
    public void Cancelling_a_pending_order_does_not_refund()
    {
        var order = new Order();

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Equal(0, order.RefundCount);
    }

    [Fact]
    public void Cancelling_a_paid_order_refunds_once()
    {
        var order = new Order();
        order.Pay();

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Equal(1, order.RefundCount);
    }

    [Fact]
    public void Returning_a_delivered_order_refunds_once()
    {
        var order = DeliveredOrder();

        order.Return();

        Assert.Equal(OrderStatus.Returned, order.Status);
        Assert.Equal(1, order.RefundCount);
    }

    [Fact]
    public void Cancelling_a_returned_order_is_rejected()
    {
        var order = DeliveredOrder();
        order.Return();

        var error = Assert.Throws<InvalidOperationException>(order.Cancel);

        Assert.Equal("Cannot cancel an order that is Returned.", error.Message);
        Assert.Equal(OrderStatus.Returned, order.Status);
        Assert.Equal(1, order.RefundCount);
    }

    [Fact]
    public void Shipping_an_unpaid_order_is_rejected()
    {
        var order = new Order();

        var error = Assert.Throws<InvalidOperationException>(order.Ship);

        Assert.Equal("Cannot ship an order that is Pending.", error.Message);
        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void Cancelling_a_shipped_order_is_rejected()
    {
        var order = new Order();
        order.Pay();
        order.Ship();

        Assert.Throws<InvalidOperationException>(order.Cancel);
        Assert.Equal(OrderStatus.Shipped, order.Status);
    }
}
