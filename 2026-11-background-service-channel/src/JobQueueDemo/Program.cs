using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSingleton<JobStore>();
builder.Services.AddSingleton<IJobQueue>(sp =>
    new ChannelJobQueue(sp.GetRequiredService<IConfiguration>().GetValue("Queue:Capacity", 100)));
builder.Services.AddHostedService<JobWorker>();

var app = builder.Build();

app.MapPost("/jobs", (CreateJobRequest request, IJobQueue queue, JobStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new { error = "Name is required." });
    }

    var job = new Job(Guid.NewGuid(), request.Name);

    // Register the job before enqueueing, so the worker never sees an unknown id.
    store.Add(job);

    if (!queue.TryEnqueue(job))
    {
        store.Remove(job.Id);
        return Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable, title: "Queue is full.");
    }

    return Results.Accepted($"/jobs/{job.Id}", new { job.Id });
});

app.MapGet("/jobs/{id:guid}", (Guid id, JobStore store) =>
    store.TryGet(id, out var state) ? Results.Ok(state) : Results.NotFound());

app.Run();

public sealed record CreateJobRequest(string? Name);

public partial class Program { }
