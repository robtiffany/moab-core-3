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
    public class DigitalTwinModelController : ControllerBase
    {
        private Services.DigitalTwinModelService digitalTwinModelService;
        private readonly ILogger<DigitalTwinModelController> _logger;
        private Services.IdentityAccessService IdentityAccessService;

        public DigitalTwinModelController(IConfiguration config, ILogger<DigitalTwinModelController> logger)
        {
            _logger = logger;

            this.digitalTwinModelService = new Services.DigitalTwinModelService(config, _logger);
            this.IdentityAccessService = new Services.IdentityAccessService(config);
        }

        // GET: api/<DigitalTwinModelController>
        [HttpGet("~/api/v3/DigitalTwinModel")]
        public ActionResult Get()
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = digitalTwinModelService.GetAllDigitalTwinModels(organizationOut);
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

        // GET api/<DigitalTwinModelController>/5
        [HttpGet("~/api/v3/DigitalTwinModel/{id}")]
        public ActionResult Get(Guid id)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = digitalTwinModelService.GetDigitalTwinModel(id, organizationOut);
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

        // POST api/<DigitalTwinModelController>
        [HttpPost("~/api/v3/DigitalTwinModel")]
        public ActionResult Post([FromBody] Models.DigitalTwinModelRequest value)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {
                    var result = digitalTwinModelService.CreateDigitalTwinModel(value, organizationOut);
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
        // PUT api/<DigitalTwinModelController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        **/

        // DELETE api/<DigitalTwinModelController>/5
        [HttpDelete("~/api/v3/DigitalTwinModel/{id}")]
        public ActionResult Delete(Guid id)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {
                    var result = digitalTwinModelService.DeleteDigitalTwinModel(id, organizationOut);
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
