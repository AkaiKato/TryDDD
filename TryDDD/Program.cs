using Scalar.AspNetCore;
using TryDDD.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpClient("openapi", client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});

var app = builder.Build();

if(!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

if(app.Environment.IsDevelopment())
{
    app.MapProxiedOpenApiDocs();
    app.MapScalarApiReference(options =>
    {
        options
        .WithTitle("TryDDD")
        .AddDocument("identity", "Identity", "/openapi/identity")
        .AddDocument("cart", "Cart", "/openapi/cart")
        .AddDocument("order", "Order", "/openapi/order")
        .AddDocument("catalog", "Catalog", "/openapi/catalog");
    });
}

app.MapReverseProxy();

app.Run();