using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class CustomerService
{
    private readonly List<Customer> _customers = [];
    private readonly Lock _lock = new();

    public IReadOnlyList<Customer> GetAll()
    {
        lock (_lock)
        {
            return [.. _customers];
        }
    }

    public Customer? GetById(Guid id)
    {
        lock (_lock)
        {
            return _customers.FirstOrDefault(customer => customer.Id == id);
        }
    }

    public Customer Create(Customer customer)
    {
        lock (_lock)
        {
            customer.Id = Guid.NewGuid();
            _customers.Add(customer);
            return customer;
        }
    }

    public Customer? Update(Guid id, Customer updated)
    {
        lock (_lock)
        {
            var existing = _customers.FirstOrDefault(customer => customer.Id == id);
            if (existing is null)
            {
                return null;
            }

            existing.Name = updated.Name;
            existing.Email = updated.Email;
            return existing;
        }
    }

    public bool Delete(Guid id)
    {
        lock (_lock)
        {
            return _customers.RemoveAll(customer => customer.Id == id) > 0;
        }
    }
}
