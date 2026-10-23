namespace EShop.Web.ApiClients;

public record ProductDto(int Id, string Name, string Description, decimal Price, string? ImageUrl);

public record CustomerDto(Guid Id, string Name, string Email);

public record CartItemDto(int ProductId, string ProductName, decimal UnitPrice, int Quantity);

public record ShoppingCartDto(Guid Id, List<CartItemDto> Items, decimal Total);

public record OrderItemDto(int ProductId, string ProductName, int Quantity, decimal UnitPrice);

public record OrderDto(int Id, string CustomerName, DateTime OrderDate, string Status, List<OrderItemDto> Items, decimal Total);

public record AddCartItemDto(int ProductId, int Quantity);

public record CheckoutDto(string CustomerName);
