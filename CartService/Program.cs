using Cart.Application.Commands;
using Cart.Application.Snapshots;
using Cart.Domain.Repositories;
using Cart.Infrastructure.Data;
using Cart.Infrastructure.Repositories;
using Cart.Infrastructure.RepositoriesForSnapshots;
using CartService.Consumers;
using CartService.OpenAPI;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Shared.AspNet;
using Shared.Auth.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CartDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IGameSnapshotRepository, GameSnapshotRepository>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AddToCartCommand).Assembly);
});

builder.Services.AddSharedAuthentication(builder.Configuration);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Configure(builder.Configuration.GetSection("Kestrel"));
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GamePublishedConsumer>();
    x.AddConsumer<PriceChangedConsumer>();
    x.AddConsumer<GameRenamedConsumer>();
    x.AddConsumer<GameArchivedConsumer>();
    x.AddInMemoryInboxOutbox();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseSharedExceptionHandling();

app.MapOpenApi();
app.MapScalarApiReference();

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if(app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<CartDbContext>();
        dbContext.Database.Migrate();
    }
}

app.Run();
