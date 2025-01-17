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

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute]int id)
    {
        try
        {
            var product = repository.Get(id);

            return Ok(product);
        }
        catch (Exception e)
        {
            return NoContent();
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] string color)
    {
        if (string.IsNullOrWhiteSpace(color))
        {
            return BadRequest("The request body cannot be empty");
        }

        try
        {
            var product = repository.Get(id);
            product.Color = color;
            repository.Update(product);

            return Ok(product);
        }
        catch (Exception e)
        {
            return NoContent();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            repository.Delete(id);

            return Ok();
        }
        catch (Exception e)
        {
            return NoContent();
        }
    }

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
            product = repository.Get(model.Name);
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
            product = repository.Get(model.Name);
        }
        catch (KeyNotFoundException e)
        {
            product = repository.AddToDictionary(model);
        }

        return CreatedAtAction(nameof(CreateProductStage4), new { date = DateTime.Now }, product);
    }
}