using IdempotencyExample.Attributes;
using IdempotencyExample.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddInfrastructureLayer();
builder.Services.AddControllers();
builder.Services.AddSingleton<IdempotentAttribute>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Use(next => context =>
{
    context.Request.EnableBuffering();
    return next(context);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
