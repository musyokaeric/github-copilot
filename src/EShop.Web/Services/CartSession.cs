using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace EShop.Web.Services;

// Stores a per-browser-tab cart id in session storage so each user gets an isolated cart.
// Requires JS interop, so callers must run after the circuit connects (prerendering disabled).
public class CartSession(ProtectedSessionStorage storage)
{
    private const string Key = "cartId";
    private Guid? _cartId;

    public async ValueTask<Guid> GetCartIdAsync()
    {
        if (_cartId is { } cached)
        {
            return cached;
        }

        var stored = await storage.GetAsync<Guid>(Key);
        if (stored is { Success: true, Value: var id } && id != Guid.Empty)
        {
            _cartId = id;
            return id;
        }

        return await ResetAsync();
    }

    public async ValueTask<Guid> ResetAsync()
    {
        var id = Guid.NewGuid();
        await storage.SetAsync(Key, id);
        _cartId = id;
        return id;
    }
}
