using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace RateLimitDemo.Tests;

public class RateLimitTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RateLimitTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(b =>
            b.ConfigureAppConfiguration((_, cfg) =>
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["RateLimit:PermitLimit"] = "3",
                    ["RateLimit:WindowSeconds"] = "60"
                })));
    }

    [Fact]
    public async Task Requisicoes_dentro_do_limite_retornam_200()
    {
        var client = _factory.CreateClient();

        for (var i = 0; i < 3; i++)
        {
            var resp = await client.GetAsync("/");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }
    }

    [Fact]
    public async Task Requisicao_acima_do_limite_retorna_429_com_retry_after()
    {
        var client = _factory.CreateClient();

        for (var i = 0; i < 3; i++) await client.GetAsync("/");
        var resp = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.TooManyRequests, resp.StatusCode);
        Assert.True(resp.Headers.Contains("Retry-After"));
    }

    [Fact]
    public async Task Health_nao_tem_limite()
    {
        var client = _factory.CreateClient();

        for (var i = 0; i < 10; i++)
        {
            var resp = await client.GetAsync("/health");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }
    }
}
