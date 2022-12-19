using User_Service.Interfaces.IServices;

namespace User_Service.Services
{
    public class UserSyncService:IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;

        private Timer _timer = null;

        public UserSyncService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SyncCaregivers, null, 1000, 100000);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void SyncCaregivers(object? state)
        {
            _ = DoSyncCaregivers();
        }

        private async Task DoSyncCaregivers()
        {
            using var scope = _serviceProvider.CreateScope();
            var scopedOrganizationService =
            scope.ServiceProvider
                    .GetRequiredService<IOrganisationService>();

            var organizations = scopedOrganizationService.GetAll();

            foreach (var organization in organizations)
            {
                try
                {
                    await FetchCaregivers(organization.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private async Task FetchCaregivers(string tenantId)
        {
            using var scope = _serviceProvider.CreateScope();
            var scopedCaregiverService =
                scope.ServiceProvider
                    .GetRequiredService<IUserService>();

            await scopedCaregiverService.FetchCaregiversFromAzureGraph(tenantId);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _timer.Dispose();
        }
    }
}
