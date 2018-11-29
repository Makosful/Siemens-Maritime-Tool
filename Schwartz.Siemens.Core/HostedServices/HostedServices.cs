using Hangfire;
using Schwartz.Siemens.Core.DomainServices.Repositories;
using System.Linq;

namespace Schwartz.Siemens.Core.HostedServices
{
    public class HostedServices : IHostedService
    {
        public HostedServices(IRigRepository rigRepository)
        {
            RigRepository = rigRepository;
        }

        private IRigRepository RigRepository { get; }

        public void StartHostedServices()
        {
            // RecurringJob.AddOrUpdate(() => TimedRigUpdates(), Cron.HourInterval(12));
            RecurringJob.AddOrUpdate("location-update", () => TimedRigUpdates(), Cron.Minutely);
        }

        private void TimedRigUpdates()
        {
            var rigs = RigRepository.ReadAll();
            RigRepository.UpdatePositions(rigs.Select(r => r.Id));
        }
    }
}