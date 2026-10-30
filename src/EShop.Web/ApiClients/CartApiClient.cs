namespace EShop.Web.ApiClients;

public class CartApiClient(HttpClient http)
{
    public async Task<ShoppingCartDto?> GetCartAsync(Guid cartId, CancellationToken ct = default) =>
        await http.GetFromJsonAsync<ShoppingCartDto>($"/api/carts/{cartId}", ct);

    public async Task<ShoppingCartDto?> AddItemAsync(Guid cartId, int productId, int quantity = 1, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync($"/api/carts/{cartId}/items", new AddCartItemDto(productId, quantity), ct);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<ShoppingCartDto>(ct) : null;
    }

    public async Task<ShoppingCartDto?> RemoveItemAsync(Guid cartId, int productId, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"/api/carts/{cartId}/items/{productId}", ct);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<ShoppingCartDto>(ct) : null;
    }

    // Returns the created order, or null with an error message when checkout fails.
    public async Task<(OrderDto? Order, string? Error)> CheckoutAsync(Guid cartId, string customerName, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync($"/api/carts/{cartId}/checkout", new CheckoutDto(customerName), ct);
        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<OrderDto>(ct), null);
        }

        var error = response.StatusCode == System.Net.HttpStatusCode.BadRequest
            ? (await response.Content.ReadAsStringAsync(ct)).Trim('"')
            : "Cart not found.";
        return (null, error);
    }
}
