using Hangfire;
using Schwartz.Siemens.Core.DomainServices.Repositories;
using System.Linq;

// ReSharper disable MemberCanBePrivate.Global

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
            RecurringJob.AddOrUpdate(() => TimedRigUpdates(), Cron.HourInterval(12));
        }

        public void TimedRigUpdates()
        {
            var rigs = RigRepository.ReadAll();
            RigRepository.UpdatePositions(rigs.Select(r => r.Id));
        }
    }
}