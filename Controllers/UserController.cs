using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MoabCore3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private Services.UserService userService;
        private readonly ILogger<UserController> _logger;
        private Services.IdentityAccessService IdentityAccessService;

        public UserController(IConfiguration config, ILogger<UserController> logger)
        {
            _logger = logger;

            this.userService = new Services.UserService(config, _logger);
            this.IdentityAccessService = new Services.IdentityAccessService(config, _logger);
        }

        // GET: api/<UserController>
        [HttpGet("~/api/v3/User")]
        public ActionResult Get()
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = userService.GetAllUsers(organizationOut);
                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(result);
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        // GET api/<UserController>/5
        [HttpGet("~/api/v3/User/{id}")]
        public ActionResult Get(Guid id)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = userService.GetUser(id, organizationOut);
                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(result);
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        // POST api/<UserController>
        [HttpPost("~/api/v3/User")]
        public ActionResult Post([FromBody] Models.UserRequest value)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {
                    var result = userService.CreateUser(value, organizationOut);
                    if (result == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Ok(result);
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        /**
        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        **/

        // DELETE api/<UserController>/5
        [HttpDelete("~/api/v3/User/{id}")]
        public ActionResult Delete(Guid id)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {
                    var result1 = userService.DeleteUser(id, organizationOut);
                    if (result1 == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Ok(result1);
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }


    }
}
