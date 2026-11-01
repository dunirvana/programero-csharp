using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var limit = builder.Configuration.GetValue("RateLimit:PermitLimit", 5);
var windowSeconds = builder.Configuration.GetValue("RateLimit:WindowSeconds", 10);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Limite por cliente (IP), janela fixa
    options.AddPolicy("por-ip", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonimo",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = limit,
                Window = TimeSpan.FromSeconds(windowSeconds),
                QueueLimit = 0
            }));

    options.OnRejected = (context, _) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)retry.TotalSeconds).ToString();
        }
        return ValueTask.CompletedTask;
    };
});

var app = builder.Build();

app.UseRateLimiter();

app.MapGet("/", () => Results.Ok(new { mensagem = "ola", hora = DateTime.UtcNow }))
   .RequireRateLimiting("por-ip");

app.MapGet("/health", () => Results.Ok("ok")); // sem limite

app.Run();

public partial class Program { }
