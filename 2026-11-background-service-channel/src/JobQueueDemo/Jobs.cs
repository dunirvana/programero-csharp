using System.Collections.Concurrent;
using System.Threading.Channels;

public enum JobStatus
{
    Queued,
    Processing,
    Completed,
    Failed
}

public sealed record Job(Guid Id, string Name);

public sealed record JobState(Guid Id, string Name, JobStatus Status);

public interface IJobQueue
{
    bool TryEnqueue(Job job);

    IAsyncEnumerable<Job> DequeueAllAsync(CancellationToken cancellationToken);
}

public sealed class ChannelJobQueue : IJobQueue
{
    private readonly Channel<Job> _channel;

    public ChannelJobQueue(int capacity)
    {
        _channel = Channel.CreateBounded<Job>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true
        });
    }

    // Returns false immediately when the queue is full, instead of waiting.
    public bool TryEnqueue(Job job) => _channel.Writer.TryWrite(job);

    public IAsyncEnumerable<Job> DequeueAllAsync(CancellationToken cancellationToken) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}

public sealed class JobStore
{
    private readonly ConcurrentDictionary<Guid, JobState> _jobs = new();

    public void Add(Job job) => _jobs[job.Id] = new JobState(job.Id, job.Name, JobStatus.Queued);

    public void Remove(Guid id) => _jobs.TryRemove(id, out _);

    public void SetStatus(Guid id, JobStatus status) =>
        _jobs.AddOrUpdate(id, _ => throw new KeyNotFoundException($"Job {id} not found."),
            (_, current) => current with { Status = status });

    public bool TryGet(Guid id, out JobState? state) => _jobs.TryGetValue(id, out state);
}
