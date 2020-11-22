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
    public class OrganizationController : ControllerBase
    {
        private Services.OrganizationService organizationService;
        private readonly ILogger<OrganizationController> _logger;

        //private Services.IdentityAccessService IdentityAccessService;

        public OrganizationController(IConfiguration config)
        {
            this.organizationService = new Services.OrganizationService(config, _logger);
            //this.IdentityAccessService = new Services.IdentityAccessService(config);
        }

        /**
        // GET: api/<OrganizationController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        **/

        /**
        // GET api/<OrganizationController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        **/

        // POST api/<OrganizationController>
        [HttpPost("~/api/v3/Organization")]
        public ActionResult Post([FromBody] Models.OrganizationRequest value)
        {
            var result = organizationService.Initialize(value);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }
        }

        /**
        // PUT api/<OrganizationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        **/

        /**
        // DELETE api/<OrganizationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {

        }
        **/

    }
}
