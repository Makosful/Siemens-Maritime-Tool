using Schwartz.Siemens.Core.DomainServices.Repositories;
using Schwartz.Siemens.Core.Entities.UserBase;
using System.Collections.Generic;
using System.Linq;

namespace Schwartz.Siemens.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(MaritimeContext context)
        {
            Context = context;
        }

        private MaritimeContext Context { get; }

        public User Read(int id)
        {
            return Context.Users.FirstOrDefault(u => u.Id == id);
        }

        public IEnumerable<User> ReadAll()
        {
            return Context.Users;
        }

        public User Create(User item)
        {
            var user = Context.Users.Add(item).Entity;
            Context.SaveChanges();
            return user;
        }

        public User Update(int id, User item)
        {
            item.Id = id;
            var user = Context.Users.Update(item).Entity;
            Context.SaveChanges();
            return user;
        }

        public User Delete(int id)
        {
            var user = Read(id);
            Context.Users.Remove(user);
            Context.SaveChanges();
            return user;
        }
    }
}