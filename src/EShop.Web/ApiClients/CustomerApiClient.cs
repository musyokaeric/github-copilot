namespace EShop.Web.ApiClients;

public class CustomerApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<CustomerDto>> GetCustomersAsync(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<List<CustomerDto>>("/api/customers", ct) ?? [];

    public Task<CustomerDto?> GetCustomerAsync(Guid id, CancellationToken ct = default) =>
        http.GetFromJsonAsync<CustomerDto>($"/api/customers/{id}", ct);

    public async Task<CustomerDto?> CreateCustomerAsync(CustomerDto customer, CancellationToken ct = default)
    {
        using var response = await http.PostAsJsonAsync("/api/customers", customer, ct);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<CustomerDto>(ct)
            : null;
    }

    public async Task<CustomerDto?> UpdateCustomerAsync(Guid id, CustomerDto customer, CancellationToken ct = default)
    {
        using var response = await http.PutAsJsonAsync($"/api/customers/{id}", customer, ct);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<CustomerDto>(ct)
            : null;
    }

    public async Task<bool> DeleteCustomerAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await http.DeleteAsync($"/api/customers/{id}", ct);
        return response.IsSuccessStatusCode;
    }
}
