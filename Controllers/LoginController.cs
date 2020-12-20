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
    public class LoginController : ControllerBase
    {
        private Services.LoginService loginService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IConfiguration config, ILogger<LoginController> logger)
        {
            _logger = logger;

            this.loginService = new Services.LoginService(config, _logger);
            //this.IdentityAccessService = new Services.IdentityAccessService(config);
        }

        /**
        // GET: api/<LoginController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        **/

        // POST api/<LoginController>
        [HttpPost("~/api/v3/Login")]
        public ActionResult Post([FromBody] Models.LoginRequest value)
        {
            var result = loginService.Login(value.EmailAddress, value.Password);
            if (result == null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok(result);
            }
        }

        /**
        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        **/
    }
}
