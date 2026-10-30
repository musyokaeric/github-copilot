using EShop.ApiService.Models;

namespace EShop.ApiService.Services;

public class ProductService
{
    private readonly List<Product> _products;
    private readonly Lock _lock = new();
    private int _nextId;

    public ProductService()
    {
        _products =
        [
            new() { Id = 1, Name = "Wireless Noise-Cancelling Headphones", Description = "Over-ear Bluetooth headphones with 30-hour battery life and active noise cancellation.", Price = 199.99m, ImageUrl = "https://example.com/images/headphones.jpg" },
            new() { Id = 2, Name = "Mechanical Keyboard", Description = "Compact 75% mechanical keyboard with hot-swappable switches and RGB backlight.", Price = 89.50m, ImageUrl = "https://example.com/images/keyboard.jpg" },
            new() { Id = 3, Name = "4K Webcam", Description = "Ultra HD webcam with autofocus, dual microphones and privacy shutter.", Price = 129.00m, ImageUrl = "https://example.com/images/webcam.jpg" },
            new() { Id = 4, Name = "Ergonomic Office Chair", Description = "Adjustable mesh-back chair with lumbar support and 4D armrests.", Price = 329.95m, ImageUrl = "https://example.com/images/chair.jpg" },
            new() { Id = 5, Name = "Portable SSD 1TB", Description = "USB-C external solid state drive with read speeds up to 1,050 MB/s.", Price = 109.99m, ImageUrl = "https://example.com/images/ssd.jpg" }
        ];
        _nextId = _products.Max(p => p.Id) + 1;
    }

    public IReadOnlyList<Product> GetAll()
    {
        lock (_lock)
        {
            return [.. _products];
        }
    }

    public Product? GetById(int id)
    {
        lock (_lock)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }
    }

    public Product Create(Product product)
    {
        lock (_lock)
        {
            product.Id = _nextId++;
            _products.Add(product);
            return product;
        }
    }

    public Product? Update(int id, Product updated)
    {
        lock (_lock)
        {
            var existing = _products.FirstOrDefault(p => p.Id == id);
            if (existing is null)
            {
                return null;
            }

            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.Price = updated.Price;
            existing.ImageUrl = updated.ImageUrl;
            return existing;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            return _products.RemoveAll(p => p.Id == id) > 0;
        }
    }
}
