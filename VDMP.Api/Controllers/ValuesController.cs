using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace VDMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new[] {"value1", "value2"};
        }

        // GET api/values/5
        [HttpGet("{Id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{Id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{Id}")]
        public void Delete(int id)
        {
        }
    }
}