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

        /// <summary>
        /// Finds a single user based on their ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User Read(int id)
        {
            return Context.Users.FirstOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Retrieves a list of all the users stored in the database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> ReadAll()
        {
            return Context.Users;
        }

        /// <summary>
        /// Creates a new user in the database
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public User Create(User item)
        {
            var user = Context.Users.Add(item).Entity;
            Context.SaveChanges();
            return user;
        }

        /// <summary>
        /// Updates the information of the User with the specified ID with the given information in the User entity
        /// </summary>
        /// <param name="item">New information to overwrite the old</param>
        /// <returns></returns>
        public User Update(User item)
        {
            var user = Context.Users.Update(item).Entity;
            Context.SaveChanges();
            return user;
        }

        /// <summary>
        /// Deletes an User with the specified ID
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public User Delete(User item)
        {
            Context.Users.Remove(item);
            Context.SaveChanges();
            return item;
        }
    }
}