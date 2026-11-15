using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

public class JobQueueTests
{
    public sealed record CreateJobResponse(Guid Id);

    public sealed record JobResponse(Guid Id, string Name, string Status);

    // One factory per test: the queue and the job store live in memory.
    private static WebApplicationFactory<Program> CreateFactory(int delayMs, int capacity = 100) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, config) =>
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Worker:DelayMs"] = delayMs.ToString(),
                    ["Queue:Capacity"] = capacity.ToString()
                })));

    private static async Task<Guid> CreateJobAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync("/jobs", new { name });
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CreateJobResponse>();
        Assert.NotNull(body);
        return body!.Id;
    }

    private static async Task<JobResponse> WaitForStatusAsync(HttpClient client, Guid id, string expected)
    {
        var deadline = DateTime.UtcNow.AddSeconds(5);
        JobResponse? last = null;

        while (DateTime.UtcNow < deadline)
        {
            last = await client.GetFromJsonAsync<JobResponse>($"/jobs/{id}");
            if (last?.Status == expected)
            {
                return last;
            }

            await Task.Delay(20);
        }

        throw new InvalidOperationException(
            $"Job {id} did not reach status {expected}. Last status: {last?.Status}");
    }

    [Fact]
    public async Task Post_ValidJob_ReturnsAcceptedWithLocation()
    {
        await using var factory = CreateFactory(delayMs: 0);
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/jobs", new { name = "report" });

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.StartsWith("/jobs/", response.Headers.Location!.OriginalString);
    }

    [Fact]
    public async Task Post_EmptyName_ReturnsBadRequest()
    {
        await using var factory = CreateFactory(delayMs: 0);
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/jobs", new { name = "  " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_UnknownJob_ReturnsNotFound()
    {
        await using var factory = CreateFactory(delayMs: 0);
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/jobs/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Job_IsProcessedByWorker_AndCompletes()
    {
        await using var factory = CreateFactory(delayMs: 50);
        var client = factory.CreateClient();

        var id = await CreateJobAsync(client, "report");
        var job = await WaitForStatusAsync(client, id, "Completed");

        Assert.Equal("report", job.Name);
    }

    [Fact]
    public async Task Worker_KeepsRunning_AfterJobFailure()
    {
        await using var factory = CreateFactory(delayMs: 10);
        var client = factory.CreateClient();

        var failingId = await CreateJobAsync(client, "fail");
        var healthyId = await CreateJobAsync(client, "report");

        await WaitForStatusAsync(client, failingId, "Failed");
        await WaitForStatusAsync(client, healthyId, "Completed");
    }

    [Fact]
    public async Task Post_WhenQueueIsFull_ReturnsServiceUnavailable()
    {
        await using var factory = CreateFactory(delayMs: 3000, capacity: 1);
        var client = factory.CreateClient();

        // The worker takes the first job and stays busy with it.
        var firstId = await CreateJobAsync(client, "first");
        await WaitForStatusAsync(client, firstId, "Processing");

        // The second job fills the queue (capacity 1).
        await CreateJobAsync(client, "second");

        // The third one has no room left.
        var response = await client.PostAsJsonAsync("/jobs", new { name = "third" });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }
}
