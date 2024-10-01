using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class CronJobService : BackgroundService
{
    private readonly ILogger<CronJobService> _logger;
    private Timer _timer;

    public CronJobService(ILogger<CronJobService> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CronJobService running.");

        // Set the timer to execute the task periodically
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); // Runs every 30 minutes

        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        _logger.LogInformation("CronJobService is working at: {time}", DateTimeOffset.Now);

        // Add your recurring task logic here
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CronJobService is stopping.");
        await base.StopAsync(stoppingToken);
    }
}
