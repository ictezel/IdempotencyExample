using IdempotencyExample.Attributes;
using IdempotencyExample.Repository;
using Microsoft.AspNetCore.Mvc;

namespace IdempotencyExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ILogger<ProductController> logger, IProductRepository repository) : ControllerBase
{
    private readonly ILogger<ProductController> logger = logger;
    private readonly IProductRepository repository = repository;

    [HttpPost("stages/v1")]
    public async Task<IActionResult> CreateProductStage1([FromBody] ProductModel model)
    {
        if (model == null)
        {
            return BadRequest("The request body cannot be empty");
        }

        var product = repository.AddToList(model);

        return CreatedAtAction(nameof(CreateProductStage1), new { date = DateTime.Now }, product);
    }

    [HttpPost("stages/v2")]
    public async Task<IActionResult> CreateProductStage2([FromBody] ProductModel model)
    {
        if (model == null)
        {
            return BadRequest("The request body cannot be empty");
        }

        Product product;
        try
        {
            product = repository.AddToDictionary(model);
        }
        catch (Exception e)
        {
            return BadRequest("Already present in the DB.");
        }

        return CreatedAtAction(nameof(CreateProductStage2), new { date = DateTime.Now }, product);
    }

    [HttpPost("stages/v3")]
    public async Task<IActionResult> CreateProductStage3([FromBody] ProductModel model)
    {
        if (model == null)
        {
            return BadRequest("The request body cannot be empty");
        }

        Product product;
        try
        {
            product = repository.GetProduct(model.Name);
        }
        catch (KeyNotFoundException e)
        {
            product = repository.AddToDictionary(model);
        }

        return CreatedAtAction(nameof(CreateProductStage3), new { date = DateTime.Now }, product);
    }

    [HttpPost("stages/v4")]
    [Idempotent(KeyValues = ["name"])]
    public async Task<IActionResult> CreateProductStage4([FromBody] ProductModel model)
    {
        if (model == null)
        {
            return BadRequest("The request body cannot be empty");
        }

        Product product;
        try
        {
            product = repository.GetProduct(model.Name);
        }
        catch (KeyNotFoundException e)
        {
            product = repository.AddToDictionary(model);
        }

        return CreatedAtAction(nameof(CreateProductStage4), new { date = DateTime.Now }, product);
    }
}