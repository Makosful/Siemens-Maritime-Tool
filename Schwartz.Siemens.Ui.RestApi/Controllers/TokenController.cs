using Microsoft.AspNetCore.Mvc;
using Schwartz.Siemens.Core.ApplicationServices;
using Schwartz.Siemens.Core.DomainServices.Repositories;
using Schwartz.Siemens.Ui.RestApi.Auth;
using System;
using System.Linq;

namespace Schwartz.Siemens.Ui.RestApi.Controllers
{
    [Route("/[controller]")]
    public class TokenController : Controller
    {
        private IUserRepository UserRepository { get; }
        private IAuthenticationHelper AuthenticationHelper { get; }

        public TokenController(IUserRepository userRepository, IAuthenticationHelper authenticationHelper)
        {
            UserRepository = userRepository;
            AuthenticationHelper = authenticationHelper;
        }

        [HttpPost]
        public ActionResult Login([FromBody] LoginModel model)
        {
            var user = UserRepository.ReadAll().FirstOrDefault(u => u.Username.Equals(model.Username, StringComparison.CurrentCultureIgnoreCase));
            if (user == null) return Unauthorized();

            if (!AuthenticationHelper.VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized();

            return Ok(new
            {
                username = user.Username,
                token = AuthenticationHelper.GenerateToken(user)
            });
        }
    }
}