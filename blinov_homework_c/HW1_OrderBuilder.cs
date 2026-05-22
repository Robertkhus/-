using System.Numerics;

namespace HW1;

public class Order<T> where T : INumber<T>
{
    public int Id { get; init; }
    public T Price { get; set; }
}

public interface IIdStep
{
    IPriceStep<T> SetId<T>(int id) where T : INumber<T>;
}

public interface IPriceStep<T> where T : INumber<T>
{
    IFinalStep<T> SetBasePrice(T price);
}

public interface IFinalStep<T> where T : INumber<T>
{
    Order<T> Build();
}

public class OrderBuilder<T> : IIdStep, IPriceStep<T>, IFinalStep<T> where T : INumber<T>
{
    private int _id;
    private T _price = T.Zero;

    public static IIdStep Start() => new OrderBuilder<T>();

    IPriceStep<TType> IIdStep.SetId<TType>(int id)
    {
        var builder = new OrderBuilder<TType> { _id = id };
        return builder;
    }

    public IFinalStep<T> SetBasePrice(T price)
    {
        _price = price;
        return this;
    }

    public Order<T> Build() => new Order<T> { Id = _id, Price = _price };
}

public abstract class OrderHandler<T> where T : INumber<T>
{
    protected OrderHandler<T>? Next;

    public OrderHandler<T> SetNext(OrderHandler<T> next)
    {
        Next = next;
        return next;
    }

    public abstract void Process(Order<T> order);
}

public class DiscountHandler<T> : OrderHandler<T> where T : INumber<T>
{
    private readonly T _discount;

    public DiscountHandler(T discount) => _discount = discount;

    public override void Process(Order<T> order)
    {
        order.Price -= _discount;
        Next?.Process(order);
    }
}

public class TaxHandler<T> : OrderHandler<T> where T : INumber<T>
{
    private readonly double _taxRate;

    public TaxHandler(double taxRate) => _taxRate = taxRate;

    public override void Process(Order<T> order)
    {
        order.Price = order.Price * T.CreateChecked(_taxRate);
        Next?.Process(order);
    }
}

public class ValidationHandler<T> : OrderHandler<T> where T : INumber<T>
{
    public override void Process(Order<T> order)
    {
        if (order.Price < T.Zero)
            throw new InvalidOperationException($"Цена не может быть отрицательной: {order.Price}");
        Next?.Process(order);
    }
}

class Program
{
    static void Main()
    {
        var decimalOrder = new OrderBuilder<decimal>()
            .SetId(1)
            .SetBasePrice(1000m)
            .Build();

        var discountHandler = new DiscountHandler<decimal>(100m);
        var taxHandler = new TaxHandler<decimal>(1.2);
        var validationHandler = new ValidationHandler<decimal>();
        discountHandler.SetNext(taxHandler).SetNext(validationHandler);

        discountHandler.Process(decimalOrder);
        Console.WriteLine($"Order<decimal> Id={decimalOrder.Id}, Price={decimalOrder.Price}");

        var doubleOrder = new OrderBuilder<double>()
            .SetId(2)
            .SetBasePrice(500.0)
            .Build();

        var discountD = new DiscountHandler<double>(50.0);
        var taxD = new TaxHandler<double>(1.2);
        var validD = new ValidationHandler<double>();
        discountD.SetNext(taxD).SetNext(validD);

        discountD.Process(doubleOrder);
        Console.WriteLine($"Order<double> Id={doubleOrder.Id}, Price={doubleOrder.Price}");
    }
}
