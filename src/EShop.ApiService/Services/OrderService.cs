using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class OrderService
{
    private readonly List<Order> _orders;
    private readonly Lock _lock = new();
    private int _nextId;

    public OrderService()
    {
        _orders =
        [
            new()
            {
                Id = 1,
                CustomerName = "Alice Johnson",
                OrderDate = DateTime.UtcNow.AddDays(-3),
                Status = OrderStatus.Shipped,
                Items =
                [
                    new() { ProductId = 1, ProductName = "Wireless Noise-Cancelling Headphones", Quantity = 1, UnitPrice = 199.99m },
                    new() { ProductId = 5, ProductName = "Portable SSD 1TB", Quantity = 2, UnitPrice = 109.99m }
                ]
            },
            new()
            {
                Id = 2,
                CustomerName = "Brian Otieno",
                OrderDate = DateTime.UtcNow.AddDays(-1),
                Status = OrderStatus.Pending,
                Items =
                [
                    new() { ProductId = 2, ProductName = "Mechanical Keyboard", Quantity = 1, UnitPrice = 89.50m }
                ]
            }
        ];
        _nextId = _orders.Max(o => o.Id) + 1;
    }

    public IReadOnlyList<Order> GetAll()
    {
        lock (_lock)
        {
            return [.. _orders];
        }
    }

    public Order? GetById(int id)
    {
        lock (_lock)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
        }
    }

    public Order Create(Order order)
    {
        lock (_lock)
        {
            order.Id = _nextId++;
            order.OrderDate = DateTime.UtcNow;
            _orders.Add(order);
            return order;
        }
    }

    public Order? Update(int id, Order updated)
    {
        lock (_lock)
        {
            var existing = _orders.FirstOrDefault(o => o.Id == id);
            if (existing is null)
            {
                return null;
            }

            existing.CustomerName = updated.CustomerName;
            existing.Status = updated.Status;
            existing.Items = updated.Items;
            return existing;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            return _orders.RemoveAll(o => o.Id == id) > 0;
        }
    }
}
