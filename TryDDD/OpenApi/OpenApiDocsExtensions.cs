using System.Text.Json.Nodes;

namespace TryDDD.OpenApi
{
    internal static class OpenApiDocsExtensions
    {
        public static void MapProxiedOpenApiDocs(this WebApplication app)
        {
            var identityUrl = app.Configuration["OpenApiDocs:Identity"]
                ?? throw new InvalidOperationException("OpenApiDocs:Identity is missing");
            var cartUrl = app.Configuration["OpenApiDocs:Cart"]
                ?? throw new InvalidOperationException("OpenApiDocs:Cart is missing");
            var orderUrl = app.Configuration["OpenApiDocs:Order"]
                ?? throw new InvalidOperationException("OpenApiDocs:Order is missing");
            var catalogtUrl = app.Configuration["OpenApiDocs:Catalog"]
                ?? throw new InvalidOperationException("OpenApiDocs:Catalog is missing");

            app.MapGet("/openapi/identity", (IHttpClientFactory httpFactory, HttpContext ctx, CancellationToken ct)
                => ProxyAsync(httpFactory, identityUrl, ctx, ct));

            app.MapGet("/openapi/cart", (IHttpClientFactory httpFactory, HttpContext ctx, CancellationToken ct)
                => ProxyAsync(httpFactory, cartUrl, ctx, ct));

            app.MapGet("/openapi/order", (IHttpClientFactory httpFactory, HttpContext ctx, CancellationToken ct)
                => ProxyAsync(httpFactory, orderUrl, ctx, ct));

            app.MapGet("/openapi/catalog", (IHttpClientFactory httpFactory, HttpContext ctx, CancellationToken ct)
                => ProxyAsync(httpFactory, catalogtUrl, ctx, ct));
        }

        private static async Task<IResult> ProxyAsync(
            IHttpClientFactory httpFactory, string backendUrl,
            HttpContext ctx, CancellationToken ct)
        {
            try
            {
                var http = httpFactory.CreateClient("openapi");
                var json = await http.GetStringAsync(backendUrl, ct);
                var node = JsonNode.Parse(json);
                if(node is JsonNode obj)
                {
                    var gatewayUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host.Value}";
                    obj["servers"] = new JsonArray(new JsonObject { ["url"] = gatewayUrl });
                }

                return Results.Content(node!.ToJsonString(), contentType: "application/json");
            }
            catch (HttpRequestException)
            {
                return Results.Problem("OpenAPI backend is unavailable", statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested)
            {
                return Results.Problem("Request was cancelled", statusCode: StatusCodes.Status408RequestTimeout);
            }
        }
    } 
}
