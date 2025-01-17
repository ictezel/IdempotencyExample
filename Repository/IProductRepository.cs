namespace IdempotencyExample.Repository;

public interface IProductRepository
{
    Product AddToList(ProductModel model);
    Product AddToDictionary(ProductModel model);
    Product Get(string name);
    Product Get(int id);
    void Update(Product product);
    void Delete(int id);
}

public class ProductRepository : IProductRepository
{
    private List<Product> productList = [];
    private Dictionary<string, Product> productDictionay = new();

    public Product AddToList(ProductModel model)
    {
        var newProduct = new Product(productList.Count + 1, model.Name, model.Color);
        productList.Add(newProduct);

        return newProduct;
    }

    public Product AddToDictionary(ProductModel model)
    {
        var newProduct = new Product(productDictionay.Count + 1, model.Name, model.Color);
        productDictionay.Add(model.Name, newProduct);

        return newProduct;
    }

    public Product Get(string name)
    {
        return productDictionay[name];
    }

    public Product Get(int id)
    {
        return productDictionay.ElementAt(id - 1).Value;
    }

    public void Update(Product product)
    {
        productDictionay[product.Name] = product;
    }

    public void Delete(int id)
    {
        var entry = productDictionay.ElementAt(id - 1);
        productDictionay.Remove(entry.Key);
    }
}

public static class InfrastructureExtensions
{
    public static void AddInfrastructureLayer(this IServiceCollection services)
    {
        services.AddSingleton<IProductRepository, ProductRepository>();
    }
}