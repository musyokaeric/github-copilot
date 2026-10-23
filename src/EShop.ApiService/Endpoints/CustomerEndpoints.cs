using EShop.ApiService.Models;
using EShop.ApiService.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EShop.ApiService.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers").WithTags("Customers");

        group.MapGet("/", (CustomerService service) => TypedResults.Ok(service.GetAll()))
            .WithName("GetCustomers");

        group.MapGet("/{id:guid}", Results<Ok<Customer>, NotFound> (Guid id, CustomerService service) =>
                service.GetById(id) is { } customer ? TypedResults.Ok(customer) : TypedResults.NotFound())
            .WithName("GetCustomerById");

        group.MapPost("/", (Customer customer, CustomerService service) =>
            {
                var created = service.Create(customer);
                return TypedResults.CreatedAtRoute(created, "GetCustomerById", new { id = created.Id });
            })
            .WithName("CreateCustomer");

        group.MapPut("/{id:guid}", Results<Ok<Customer>, NotFound> (Guid id, Customer customer, CustomerService service) =>
                service.Update(id, customer) is { } updated ? TypedResults.Ok(updated) : TypedResults.NotFound())
            .WithName("UpdateCustomer");

        group.MapDelete("/{id:guid}", Results<NoContent, NotFound> (Guid id, CustomerService service) =>
                service.Delete(id) ? TypedResults.NoContent() : TypedResults.NotFound())
            .WithName("DeleteCustomer");

        return app;
    }
}
