using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RateLimitDemo.Tests;

public class RateLimitTests : IDisposable
{
    // Uma instancia nova da API por teste, para que o contador de um
    // teste nao interfira no outro.
    private readonly WebApplicationFactory<Program> _factory;

    public RateLimitTests()
    {
        // O Program le o limite ao montar o builder, entao a configuracao
        // precisa existir antes do host subir (variaveis de ambiente resolvem isso).
        Environment.SetEnvironmentVariable("RateLimit__PermitLimit", "3");
        Environment.SetEnvironmentVariable("RateLimit__WindowSeconds", "60");
        _factory = new WebApplicationFactory<Program>();
    }

    public void Dispose() => _factory.Dispose();

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
