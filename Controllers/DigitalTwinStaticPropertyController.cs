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
    public class DigitalTwinStaticPropertyController : ControllerBase
    {
        private Services.DigitalTwinStaticPropertyService digitalTwinStaticPropertyService;
        private readonly ILogger<DigitalTwinStaticPropertyController> _logger;
        private Services.IdentityAccessService IdentityAccessService;

        public DigitalTwinStaticPropertyController(IConfiguration config, ILogger<DigitalTwinStaticPropertyController> logger)
        {
            _logger = logger;

            this.digitalTwinStaticPropertyService = new Services.DigitalTwinStaticPropertyService(config, _logger);
            this.IdentityAccessService = new Services.IdentityAccessService(config);
        }

        // GET: api/<DigitalTwinStaticPropertyController>
        [HttpGet("~/api/v3/DigitalTwinStaticProperty/{DigitalTwin}")]
        public ActionResult Get(Guid digitalTwin)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = digitalTwinStaticPropertyService.GetAllDigitalTwinStaticProperties(digitalTwin, organizationOut);
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

        // GET api/<DigitalTwinStaticPropertyController>/5
        [HttpGet("~/api/v3/DigitalTwinStaticProperty/{id}/{DigitalTwin}")]
        public ActionResult Get(Guid id, Guid digitalTwin)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = digitalTwinStaticPropertyService.GetDigitalTwinStaticProperty(id, digitalTwin, organizationOut);
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

        // POST api/<DigitalTwinStaticPropertyController>
        [HttpPost("~/api/v3/DigitalTwinStaticProperty")]
        public ActionResult Post([FromBody] Models.DigitalTwinStaticPropertyRequest value)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {

                    var result = digitalTwinStaticPropertyService.CreateDigitalTwinStaticProperty(value, organizationOut);
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
        // PUT api/<DigitalTwinStaticPropertyController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] string value)
        {
        }
        **/

        // DELETE api/<DigitalTwinStaticPropertyController>/5
        [HttpDelete("~/api/v3/DigitalTwinStaticProperty/{id}/{DigitalTwin}")]
        public ActionResult Delete(Guid id, Guid digitalTwin)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {

                    var result1 = digitalTwinStaticPropertyService.DeleteDigitalTwinStaticProperty(id, digitalTwin, organizationOut);
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
