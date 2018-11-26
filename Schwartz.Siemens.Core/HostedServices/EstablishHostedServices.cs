using Hangfire;
using System;

namespace Schwartz.Siemens.Core.HostedServices
{
    public class EstablishHostedServices
    {
        public EstablishHostedServices()
        {
            RecurringJob.AddOrUpdate(() => Console.WriteLine("My awesome message"), Cron.Minutely);
        }
    }
}