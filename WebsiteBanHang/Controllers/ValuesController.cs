using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            Work();
            Console.WriteLine("return roi");
            return new string[] { "value1", "value2" };
        }
        public async void Work()
        {
            string ha = await SlowTask();
            Console.WriteLine("Execution completed "+ha);
        }
        async Task<string> SlowTask()
        {
            Console.WriteLine("A");
            string a = "A";
            await Task.Delay(5000);
            a = string.Concat(a,"B");
            Console.WriteLine("B");
            await Task.Delay(5000);
            Console.WriteLine("C");
            a = string.Concat(a, "C");
            return a;
        }
        // GET api/values/5
        [HttpGet("{id}")]
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
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
