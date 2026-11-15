public sealed class JobWorker(
    IJobQueue queue,
    JobStore store,
    IConfiguration configuration,
    ILogger<JobWorker> logger) : BackgroundService
{
    private readonly TimeSpan _delay =
        TimeSpan.FromMilliseconds(configuration.GetValue("Worker:DelayMs", 1000));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (var job in queue.DequeueAllAsync(stoppingToken))
            {
                await ProcessAsync(job, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker is shutting down.");
        }
    }

    private async Task ProcessAsync(Job job, CancellationToken stoppingToken)
    {
        store.SetStatus(job.Id, JobStatus.Processing);
        logger.LogInformation("Processing job {JobId} ({JobName}).", job.Id, job.Name);

        try
        {
            await Task.Delay(_delay, stoppingToken);

            if (job.Name == "fail")
            {
                throw new InvalidOperationException("Simulated job failure.");
            }

            store.SetStatus(job.Id, JobStatus.Completed);
            logger.LogInformation("Job {JobId} completed.", job.Id);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // One bad job must not kill the worker loop.
            store.SetStatus(job.Id, JobStatus.Failed);
            logger.LogError(ex, "Job {JobId} failed.", job.Id);
        }
    }
}
