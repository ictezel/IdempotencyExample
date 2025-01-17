namespace IdempotencyExample;

public class Product(int Id, string Name, string Color)
{
    public int Id { get; init; } = Id;
    public string Name { get; init; } = Name;
    public string Color { get; set; } = Color;

}

public record ProductModel(string Name, string Color);