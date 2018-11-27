using Microsoft.EntityFrameworkCore;
using Schwartz.Siemens.Core.DomainServices;
using Schwartz.Siemens.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Schwartz.Siemens.Infrastructure.Data.Repositories
{
    public class RigRepository : IRigRepository
    {
        public RigRepository(MaritimeContext context)
        {
            Context = context;
        }

        private MaritimeContext Context { get; }

        public Rig Read(int id)
        {
            return Context.Rigs
                .Include(r => r.Location)
                .FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<Rig> ReadAll()
        {
            return Context.Rigs
                .Include(r => r.Location);
        }

        public Rig Create(Rig item)
        {
            var rig = Context.Rigs.Add(item).Entity;
            Context.SaveChanges();
            return rig;
        }

        public Rig Update(int id, Rig item)
        {
            item.Id = id;
            var rig = Context.Rigs.Add(item).Entity;
            Context.SaveChanges();
            return rig;
        }

        public Rig Delete(int id)
        {
            var rig = Read(id);
            Context.Rigs.Remove(rig);
            return rig;
        }
    }
}