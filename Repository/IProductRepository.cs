namespace IdempotencyExample.Repository;

public interface IProductRepository
{
    Product AddToList(ProductModel model);
    Product AddToDictionary(ProductModel model);
    Product GetProduct(string name);
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

    public Product GetProduct(string name)
    {
        return productDictionay[name];
    }
}

public static class InfrastructureExtensions
{
    public static void AddInfrastructureLayer(this IServiceCollection services)
    {
        services.AddSingleton<IProductRepository, ProductRepository>();
    }
}