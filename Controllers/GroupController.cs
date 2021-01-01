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
    public class GroupController : ControllerBase
    {
        private Services.GroupService groupService;
        private readonly ILogger<GroupController> _logger;
        private Services.IdentityAccessService IdentityAccessService;

        public GroupController(IConfiguration config, ILogger<GroupController> logger)
        {
            _logger = logger;

            this.groupService = new Services.GroupService(config, _logger);
            this.IdentityAccessService = new Services.IdentityAccessService(config);
        }

        // GET: api/<GroupController>
        [HttpGet("~/api/v3/Group")]
        public ActionResult Get()
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = groupService.GetAllGroups(organizationOut);
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

        // GET api/<GroupController>/5
        [HttpGet("~/api/v3/Group/{id}")]
        public ActionResult Get(Guid id)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = groupService.GetGroup(id, organizationOut);
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

        // POST api/<GroupController>
        [HttpPost("~/api/v3/Group")]
        public ActionResult Post([FromBody] Models.GroupRequest value)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {
                    var result = groupService.CreateGroup(value, organizationOut);
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
        // PUT api/<GroupController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        **/

        // DELETE api/<GroupController>/5
        [HttpDelete("~/api/v3/Group/{id}")]
        public ActionResult Delete(Guid id)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {
                    var result1 = groupService.DeleteGroup(id, organizationOut);
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
