using Microsoft.EntityFrameworkCore;
using MinimalAPIsDemo.Data;
using MinimalAPIsDemo.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DbContextClass>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//get the list of product
app.MapGet("/productlist", async (DbContextClass dbContext) =>
{
    var products = await dbContext.Product.ToListAsync();
    if (products == null)
    {
        return Results.NoContent();
    }
    return Results.Ok(products);
});

//get product by id
app.MapGet("/getproductbyid", async (int id, DbContextClass dbContext) =>
{
    var product = await dbContext.Product.FindAsync(id);
    if (product == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(product);
});

//create a new product
app.MapPost("/createproduct", async (Product product, DbContextClass dbContext) =>
{
    var result = dbContext.Product.Add(product);
    await dbContext.SaveChangesAsync();
    return Results.Ok(result.Entity);
});

//update the product
app.MapPut("/updateproduct", async (Product product, DbContextClass dbContext) =>
{
    var productDetail = await dbContext.Product.FindAsync(product.ProductId);
    if (product == null)
    {
        return Results.NotFound();
    }
    productDetail.ProductName = product.ProductName;
    productDetail.ProductDescription = product.ProductDescription;
    productDetail.ProductPrice = product.ProductPrice;
    productDetail.ProductStock = product.ProductStock;

    await dbContext.SaveChangesAsync();
    return Results.Ok(productDetail);
});

//delete the product by id
app.MapDelete("/deleteproduct/{id}", async (int id, DbContextClass dbContext) =>
{
    var product = await dbContext.Product.FindAsync(id);
    if (product == null)
    {
        return Results.NoContent();
    }
    dbContext.Product.Remove(product);
    await dbContext.SaveChangesAsync();
    return Results.Ok();
});

app.Run();
