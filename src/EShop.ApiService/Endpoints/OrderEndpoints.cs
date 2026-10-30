using EShop.ApiService.Models;
using EShop.ApiService.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EShop.ApiService.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapGet("/", (OrderService service) => TypedResults.Ok(service.GetAll()))
            .WithName("GetOrders");

        group.MapGet("/{id:int}", Results<Ok<Order>, NotFound> (int id, OrderService service) =>
                service.GetById(id) is { } order ? TypedResults.Ok(order) : TypedResults.NotFound())
            .WithName("GetOrderById");

        group.MapPost("/", (Order order, OrderService service) =>
            {
                var created = service.Create(order);
                return TypedResults.CreatedAtRoute(created, "GetOrderById", new { id = created.Id });
            })
            .WithName("CreateOrder");

        group.MapPut("/{id:int}", Results<Ok<Order>, NotFound> (int id, Order order, OrderService service) =>
                service.Update(id, order) is { } updated ? TypedResults.Ok(updated) : TypedResults.NotFound())
            .WithName("UpdateOrder");

        group.MapDelete("/{id:int}", Results<NoContent, NotFound> (int id, OrderService service) =>
                service.Delete(id) ? TypedResults.NoContent() : TypedResults.NotFound())
            .WithName("DeleteOrder");

        return app;
    }
}
