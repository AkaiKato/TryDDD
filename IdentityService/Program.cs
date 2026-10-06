using Identity.Domain.Repositories;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Repositories;
using IdentityService.OpenAPI;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Shared.AspNet;
using Shared.Auth.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IAccountRepository, AccountRepository>();

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
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        dbContext.Database.Migrate();
    }
}

app.Run();