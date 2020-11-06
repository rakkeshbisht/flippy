using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LoginAppService.Models;
using LoginAppService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LoginAppService.Controllers
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly Service _Service;
        private IConfiguration _config;

        public LoginController(IConfiguration config, Service service)
        {
            _config = config;
            _Service = service;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            var dbUser = _Service.GetUser(user);

            if (dbUser == null)
            {
                return NotFound("User not found.");
            }

            var isValid = dbUser.password == user.password;

            if (!isValid)
            {
                return BadRequest("Could not authenticate user.");
            }

            var result = _Service.GetTokenAsync(user.clientid, user.clientsecret, user.scope);

            return Ok(result.Result.AccessToken);
        }
    }
}
