using EShop.ApiService.Models;
using EShop.ApiService.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EShop.ApiService.Endpoints;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carts").WithTags("Carts");

        group.MapGet("/{cartId:guid}", (Guid cartId, CartService service) => TypedResults.Ok(service.GetOrCreate(cartId)))
            .WithName("GetCart");

        group.MapPost("/{cartId:guid}/items", Results<Ok<ShoppingCart>, NotFound, BadRequest<string>> (Guid cartId, AddCartItemRequest request, CartService service) =>
            {
                if (request.Quantity < 1)
                {
                    return TypedResults.BadRequest("Quantity must be at least 1.");
                }

                return service.AddItem(cartId, request.ProductId, request.Quantity) is { } cart
                    ? TypedResults.Ok(cart)
                    : TypedResults.NotFound();
            })
            .WithName("AddCartItem");

        group.MapDelete("/{cartId:guid}/items/{productId:int}", Results<Ok<ShoppingCart>, NotFound> (Guid cartId, int productId, CartService service) =>
                service.RemoveItem(cartId, productId) is { } cart ? TypedResults.Ok(cart) : TypedResults.NotFound())
            .WithName("RemoveCartItem");

        group.MapPost("/{cartId:guid}/checkout", Results<CreatedAtRoute<Order>, NotFound, BadRequest<string>> (Guid cartId, CheckoutRequest request, CartService service) =>
            {
                if (string.IsNullOrWhiteSpace(request.CustomerName))
                {
                    return TypedResults.BadRequest("Customer name is required.");
                }

                if (!service.Exists(cartId))
                {
                    return TypedResults.NotFound();
                }

                return service.Checkout(cartId, request.CustomerName) is { } order
                    ? TypedResults.CreatedAtRoute(order, "GetOrderById", new { id = order.Id })
                    : TypedResults.BadRequest("Cart is empty.");
            })
            .WithName("CheckoutCart");

        return app;
    }
}
