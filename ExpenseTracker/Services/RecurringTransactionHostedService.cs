namespace ExpenseTracker.Services;

public class RecurringTransactionHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RecurringTransactionHostedService> _logger;

    public RecurringTransactionHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<RecurringTransactionHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IRecurringTransactionService>();
                await service.ProcessDueTransactionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing recurring transactions");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
