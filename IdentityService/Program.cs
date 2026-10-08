using Identity.Application.Commands;
using Identity.Application.Queries;
using Identity.Domain.Heplers;
using Identity.Domain.Repositories;
using Identity.Domain.Security;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Helpers;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Security;
using IdentityService.Background;
using IdentityService.Consumers;
using IdentityService.OpenAPI;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Shared.AspNet;
using Shared.Auth.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IRefreshSessionRepository, RefreshSessionRepository>();
builder.Services.AddScoped<IOutbox, Outbox>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterAccountCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetAccountByIdQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(UnBlockAccountCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(BlockAccountCommand).Assembly);
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
    x.AddConsumer<AccountRegisteredConsumer>();
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
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        dbContext.Database.Migrate();
    }
}

app.Run();