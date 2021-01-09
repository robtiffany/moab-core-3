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
    public class DigitalTwinController : ControllerBase
    {
        private Services.DigitalTwinService digitalTwinService;
        private readonly ILogger<DigitalTwinController> _logger;
        private Services.IdentityAccessService IdentityAccessService;

        public DigitalTwinController(IConfiguration config, ILogger<DigitalTwinController> logger)
        {
            _logger = logger;

            this.digitalTwinService = new Services.DigitalTwinService(config, _logger);
            this.IdentityAccessService = new Services.IdentityAccessService(config);
        }

        // GET: api/<DigitalTwinController>
        [HttpGet("~/api/v3/DigitalTwin")]
        public ActionResult Get()
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = digitalTwinService.GetAllDigitalTwins(organizationOut);
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

        // GET api/<DigitalTwinController>/5
        [HttpGet("~/api/v3/DigitalTwin/{id}")]
        public ActionResult Get(Guid id)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = digitalTwinService.GetDigitalTwin(id, organizationOut);
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

        // POST api/<DigitalTwinController>
        [HttpPost("~/api/v3/DigitalTwin")]
        public ActionResult Post([FromBody] Models.DigitalTwinRequest value)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {
                    var result = digitalTwinService.CreateDigitalTwin(value, organizationOut);
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
        // PUT api/<DigitalTwinController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] string value)
        {
        }
        **/

        // DELETE api/<DigitalTwinController>/5
        [HttpDelete("~/api/v3/DigitalTwin/{id}")]
        public ActionResult Delete(Guid id)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {
                    var result = digitalTwinService.DeleteDigitalTwin(id, organizationOut);
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
    }
}
