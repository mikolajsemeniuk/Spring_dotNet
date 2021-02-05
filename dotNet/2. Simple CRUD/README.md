# Simple CRUD
* Create controller
* Create routes

### Create controller
Create controller SimpleController by typing in your terminal
```sh
cd API/Controllers
touch SimpleController.cs
```
### Create routes
Paste code below and check results in postman
```cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SimpleController : ControllerBase 
    {
        private static List<int> _data { get; set; }
        public SimpleController()
        {
            _data = new List<int>() { 1, 2, 3, 4, 5 };
        }


        [HttpGet]
        public ActionResult get() => Ok(new { message = "simple get done", _data });

        [HttpPost("{value}")]
        public ActionResult post(int value)
        {
            _data.Add(value);
            return Ok(new { message = "simple post done", _data });
        }

        [HttpDelete("{index}")]
        public ActionResult delete(int index)
        { 
            _data.RemoveAt(index);
            return Ok(new { message = "simple delete done", _data });
        }

        [HttpPut("{index}/{value}")]
        public ActionResult put(int index, int value)
        {
            _data.Insert(index, value);
            return Ok(new { message = "simple put done", _data });
        }

        [HttpGet("dynamic-or-object")]
        public ActionResult<dynamic> __dynamic() => Ok(new { message = "simple endpoint with dynamic declared args" });
        [HttpGet("static")]
        public ActionResult<string> __static() => "simple endpoint with static string declared args";



        //----------------------------------------------------------------
        //
        // custom methods
        //
        //----------------------------------------------------------------
        [HttpGet("custom")]
        public ActionResult Custom()
        {
            return StatusCode(303, new { response = "custom" });
        }

        [HttpGet("server-error")]
        public ActionResult ServerError()
        {
            return StatusCode(500, new { Id = 123, Name = "computer says no!" });
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest(new { response = "this was not a good request" });
        }

        [HttpGet("not-found")]
        public ActionResult<string> GetNotFound()
        {
            return NotFound(new { response = "this is not the page you are looking for" });
        }

        [HttpGet("not-pass")]
        public ActionResult<string> GetException()
        {
            return Unauthorized(new { response = "you shall not pass"});
        }

    }
}
```
