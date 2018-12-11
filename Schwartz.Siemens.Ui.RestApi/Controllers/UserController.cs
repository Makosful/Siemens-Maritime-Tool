using Microsoft.AspNetCore.Mvc;
using Schwartz.Siemens.Core.ApplicationServices;
using Schwartz.Siemens.Core.DomainServices.Repositories;
using Schwartz.Siemens.Core.Entities.UserBase;
using Schwartz.Siemens.Ui.RestApi.Auth;
using System.Linq;

namespace Schwartz.Siemens.Ui.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UserController(IUserRepository userRepository, IAuthenticationHelper authenticationHelper)
        {
            UserRepository = userRepository;
            AuthenticationHelper = authenticationHelper;
        }

        private IUserRepository UserRepository { get; }
        private IAuthenticationHelper AuthenticationHelper { get; }

        [HttpPost]
        public ActionResult<string> CreateUser([FromBody] LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Username.Trim()) || string.IsNullOrEmpty(model.Password.Trim()))
                return BadRequest();

            var users = UserRepository.ReadAll(0, 0).ToList();

            if (users.Count > 0)
                return BadRequest();

            AuthenticationHelper.CreatePasswordHash(model.Password, out var hash, out var salt);
            var user = UserRepository.Create(new User
            {
                Username = model.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
                IsAdmin = true
            });

            return AuthenticationHelper.GenerateToken(user);
        }
    }
}