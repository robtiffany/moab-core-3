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
    public class DigitalThreadEventController : ControllerBase
    {
        private Services.DigitalThreadEventService digitalThreadEventService;
        private readonly ILogger<DigitalThreadEventController> _logger;
        private Services.IdentityAccessService IdentityAccessService;

        public DigitalThreadEventController(IConfiguration config, ILogger<DigitalThreadEventController> logger)
        {
            _logger = logger;

            this.digitalThreadEventService = new Services.DigitalThreadEventService(config, _logger);
            this.IdentityAccessService = new Services.IdentityAccessService(config);
        }

        // GET: api/<DigitalThreadEventController>
        [HttpGet("~/api/v3/DigitalThreadEvent/{DigitalTwin}")]
        public ActionResult Get(Guid digitalTwin)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = digitalThreadEventService.GetAllDigitalThreadEvents(digitalTwin, organizationOut);
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

        // GET api/<DigitalThreadEventController>/5
        [HttpGet("~/api/v3/DigitalThreadEvent/{id}/{DigitalTwin}")]
        public ActionResult Get(Guid id, Guid digitalTwin)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = digitalThreadEventService.GetDigitalThreadEvent(id, digitalTwin, organizationOut);
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

        // POST api/<DigitalThreadEventController>
        [HttpPost("~/api/v3/DigitalThreadEvent")]
        public ActionResult Post([FromBody] Models.DigitalThreadEventRequest value)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {
                    var result = digitalThreadEventService.CreateDigitalThreadEvent(value, organizationOut);
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
        // PUT api/<DigitalThreadEventController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        **/

        // DELETE api/<DigitalThreadEventController>/5
        [HttpDelete("~/api/v3/DigitalThreadEvent/{id}")]
        public ActionResult Delete(Guid id)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {
                    var result1 = digitalThreadEventService.DeleteDigitalThreadEvent(id, organizationOut);
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
