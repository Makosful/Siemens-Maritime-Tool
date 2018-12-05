using Schwartz.Siemens.Core.DomainServices;
using Schwartz.Siemens.Infrastructure.Data;

namespace Schwartz.Siemens.Infrastructure.Static.Data
{
    public class SqLiteDatabase : IDatabase
    {
        public SqLiteDatabase(MaritimeContext context)
        {
            Context = context;
        }

        private MaritimeContext Context { get; }

        public void Initialize()
        {
            Context.Database.EnsureCreated();
        }
    }
}