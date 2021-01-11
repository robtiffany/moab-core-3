using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MoabCore3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        private Services.TelemetryService telemetryService;
        private readonly ILogger<TelemetryController> _logger;
        private Services.IdentityAccessService IdentityAccessService;

        public TelemetryController(IConfiguration config, ILogger<TelemetryController> logger)
        {
            _logger = logger;

            this.telemetryService = new Services.TelemetryService(config, _logger);
            this.IdentityAccessService = new Services.IdentityAccessService(config);
        }

        // GET: api/<TelemetryController>
        [HttpGet("~/api/v3/Telemetry")]
        public ActionResult Get()
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = telemetryService.GetAllTelemetry(organizationOut);
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

        // GET api/<TelemetryController>/5
        [HttpGet("~/api/v3/Telemetry/{id}")]
        public ActionResult Get(Guid id)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                var result = telemetryService.GetTelemetry(id, organizationOut);
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

        // POST api/<TelemetryController>
        [HttpPost("~/api/v3/Telemetry")]
        public ActionResult Post([FromBody] JsonElement value)
        {
            if (IdentityAccessService.IsEntityAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out Guid digitalTwinModelOut))
            {
                var result = telemetryService.CreateTelemetry(value, identityOut, organizationOut, digitalTwinModelOut);
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

        /**
        // PUT api/<TelemetryController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] string value)
        {
        }
        **/

        // DELETE api/<TelemetryController>/5
        [HttpDelete("~/api/v3/Telemetry/{id}")]
        public ActionResult Delete(Guid id)
        {
            if (IdentityAccessService.IsUserAuthorized(HttpContext, out Guid organizationOut, out Guid identityOut, out long roleOut))
            {
                if (roleOut == 1)
                {
                    var result1 = telemetryService.DeleteTelemetry(id, organizationOut);
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
