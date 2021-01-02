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
    public class DigitalTwinModelStaticPropertyController : ControllerBase
    {
        private Services.DigitalTwinModelStaticPropertyService digitalTwinModelStaticProperty;
        private readonly ILogger<DigitalTwinModelStaticPropertyController> _logger;
        private Services.IdentityAccessService IdentityAccessService;

        public DigitalTwinModelStaticPropertyController(IConfiguration config, ILogger<DigitalTwinModelStaticPropertyController> logger)
        {
            _logger = logger;

            this.digitalTwinModelStaticProperty = new Services.DigitalTwinModelStaticPropertyService(config, _logger);
            this.IdentityAccessService = new Services.IdentityAccessService(config);
        }

        // GET: api/<DigitalTwinModelStaticPropertyController>
        [HttpGet("~/api/v3/DigitalTwinModelStaticProperty/{DigitalTwinModel}")]
        public ActionResult Get(Guid digitalTwinModel)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = digitalTwinModelStaticProperty.GetAllDigitalTwinModelStaticProperties(digitalTwinModel, organizationOut);
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

        // GET api/<DigitalTwinModelStaticPropertyController>/5
        [HttpGet("~/api/v3/DigitalTwinModelStaticProperty/{id}/{DigitalTwinModel}")]
        public ActionResult Get(Guid id, Guid digitalTwinModel)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = digitalTwinModelStaticProperty.GetDigitalTwinModelStaticProperty(id, digitalTwinModel, organizationOut);
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

        // POST api/<DigitalTwinModelStaticPropertyController>
        [HttpPost("~/api/v3/DigitalTwinModelStaticProperty")]
        public ActionResult Post([FromBody] Models.DigitalTwinModelStaticPropertyRequest value)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {

                    var result = digitalTwinModelStaticProperty.CreateDigitalTwinModelStaticProperty(value, organizationOut);
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
        // PUT api/<DigitalTwinModelStaticPropertyController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        **/

        // DELETE api/<DigitalTwinModelStaticPropertyController>/5
        [HttpDelete("~/api/v3/DigitalTwinModelStaticProperty/{id}/{DigitalTwinModel}")]
        public ActionResult Delete(Guid id, Guid digitalTwinModel)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {

                    var result1 = digitalTwinModelStaticProperty.DeleteDigitalTwinModelStaticProperty(id, digitalTwinModel, organizationOut);
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
