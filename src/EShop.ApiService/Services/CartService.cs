using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class CartService(ProductService products, OrderService orders)
{
    private readonly Dictionary<Guid, ShoppingCart> _carts = [];
    private readonly Lock _lock = new();

    public ShoppingCart GetOrCreate(Guid cartId)
    {
        lock (_lock)
        {
            return Snapshot(GetOrCreateUnsafe(cartId));
        }
    }

    public ShoppingCart? AddItem(Guid cartId, int productId, int quantity)
    {
        var product = products.GetById(productId);
        if (product is null)
        {
            return null;
        }

        lock (_lock)
        {
            var cart = GetOrCreateUnsafe(cartId);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null)
            {
                cart.Items.Add(new()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = quantity
                });
            }
            else
            {
                item.Quantity += quantity;
            }

            return Snapshot(cart);
        }
    }

    public ShoppingCart? RemoveItem(Guid cartId, int productId)
    {
        lock (_lock)
        {
            if (!_carts.TryGetValue(cartId, out var cart) || cart.Items.RemoveAll(i => i.ProductId == productId) == 0)
            {
                return null;
            }

            return Snapshot(cart);
        }
    }

    // Returns null when the cart is missing or empty.
    public Order? Checkout(Guid cartId, string customerName)
    {
        lock (_lock)
        {
            if (!_carts.TryGetValue(cartId, out var cart) || cart.Items.Count == 0)
            {
                return null;
            }

            var order = orders.Create(new()
            {
                CustomerName = customerName,
                Items =
                [
                    .. cart.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    })
                ]
            });

            _carts.Remove(cartId);
            return order;
        }
    }

    public bool Exists(Guid cartId)
    {
        lock (_lock)
        {
            return _carts.ContainsKey(cartId);
        }
    }

    private static ShoppingCart Snapshot(ShoppingCart cart) => new()
    {
        Id = cart.Id,
        Items =
        [
            .. cart.Items.Select(i => new CartItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            })
        ]
    };

    private ShoppingCart GetOrCreateUnsafe(Guid cartId)
    {
        if (!_carts.TryGetValue(cartId, out var cart))
        {
            cart = new() { Id = cartId };
            _carts[cartId] = cart;
        }

        return cart;
    }
}
