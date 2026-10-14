using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Catalog.Domain.Helpers;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Helpers;
using Catalog.Infrastructure.Repositories;
using CatalogService.Background;
using CatalogService.OpenAPI;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Shared.AspNet;
using Shared.Auth.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IOutbox, Outbox>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetAllGamesQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ArchiveGameCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ChangePriceCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateGameCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(PublishGameCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(RenameGameCommand).Assembly);
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

builder.Services.AddHostedService<OutboxDispatcher>();

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
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        dbContext.Database.Migrate();
    }
}

app.Run();