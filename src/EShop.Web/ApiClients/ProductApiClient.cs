namespace EShop.Web.ApiClients;

public class ProductApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<List<ProductDto>>("/api/products", ct) ?? [];
}
