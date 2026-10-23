namespace EShop.ApiService.Models;

public class ShoppingCart
{
    public Guid Id { get; set; }
    public List<CartItem> Items { get; set; } = [];
    public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
}

public class CartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

public record AddCartItemRequest(int ProductId, int Quantity = 1);

public record CheckoutRequest(string CustomerName);
