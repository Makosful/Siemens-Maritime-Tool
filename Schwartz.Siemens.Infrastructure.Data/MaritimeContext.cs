using Microsoft.EntityFrameworkCore;

namespace Schwartz.Siemens.Infrastructure.Data
{
    public class MaritimeContext : DbContext
    {
        public MaritimeContext(DbContextOptions<MaritimeContext> options) : base(options)
        {
        }
    }
}