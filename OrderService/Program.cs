using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Application.Checkouts;
using Order.Application.Snapshots;
using Order.Domain.Repositories;
using Order.Infrastructure.Data;
using Order.Infrastructure.Repositories;
using Order.Infrastructure.RepositoriesForSnapshots;
using OrderService.Consumers;
using OrderService.OpenAPI;
using Scalar.AspNetCore;
using Shared.AspNet;
using Shared.Auth.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ILicenseKeyRepository, LicenseKeyRepository>();
builder.Services.AddScoped<IRejectedCheckoutRepository, RejectedCheckoutRepository>();
builder.Services.AddScoped<IGameSnapshotRepository, GameSnapshotRepository>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GamePublishedConsumer>();
    x.AddConsumer<PriceChangedConsumer>();
    x.AddConsumer<GameRenamedConsumer>();
    x.AddConsumer<GameArchivedConsumer>();
    x.AddEntityFrameworkOutbox<OrderDbContext>(z =>
    {
        z.UsePostgres();
        z.UseBusOutbox();
    });
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

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        dbContext.Database.Migrate();
    }
}

app.Run();

