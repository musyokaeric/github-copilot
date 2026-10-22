using EShop.ApiService.Models;
using EShop.ApiService.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EShop.ApiService.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", (ProductService service) => TypedResults.Ok(service.GetAll()))
            .WithName("GetProducts");

        group.MapGet("/{id:int}", Results<Ok<Product>, NotFound> (int id, ProductService service) =>
                service.GetById(id) is { } product ? TypedResults.Ok(product) : TypedResults.NotFound())
            .WithName("GetProductById");

        group.MapPost("/", (Product product, ProductService service) =>
            {
                var created = service.Create(product);
                return TypedResults.CreatedAtRoute(created, "GetProductById", new { id = created.Id });
            })
            .WithName("CreateProduct");

        group.MapPut("/{id:int}", Results<Ok<Product>, NotFound> (int id, Product product, ProductService service) =>
                service.Update(id, product) is { } updated ? TypedResults.Ok(updated) : TypedResults.NotFound())
            .WithName("UpdateProduct");

        group.MapDelete("/{id:int}", Results<NoContent, NotFound> (int id, ProductService service) =>
                service.Delete(id) ? TypedResults.NoContent() : TypedResults.NotFound())
            .WithName("DeleteProduct");

        return app;
    }
}
