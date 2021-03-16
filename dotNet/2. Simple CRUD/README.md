# Simple CRUD
* Create controller
* Create body

### Create controller
```cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DAPI.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger _logger;
        List<int> _users;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
            _users = new List<int>() { 1, 2, 3 };
        }

        [HttpGet]
        public ActionResult get() => Ok(new { message = "get", data = _users });

        [HttpPut("{index}/{value}")]
        public ActionResult put([FromRoute] int index, [FromRoute] int value)
        {
            _users.Insert(index, value);            
            return StatusCode(201, new { message = "put", data = _users });
        }

        [HttpPatch]
        public ActionResult patch([FromQuery] int index, [FromQuery] int value)
        {
            _users.Insert(index, value);
            return StatusCode(201, new { message = "put", data = _users });
        }

        [HttpPost]
        public ActionResult post([FromBody] SampleBody sampleBody)
        {
            var s = Request.Headers["Content-Type"];
            _logger.LogInformation("idx: " + sampleBody.index + " value: " + sampleBody.value + ", s: " + s);
            _users.Insert(sampleBody.index, sampleBody.value);
            return StatusCode(201, new { message = "post", data = _users });
        }
    }
}
```
### Create body
in `DTO/Samplebody.cs`
```cs
namespace DAPI.Controllers
{
    public class SampleBody
    {
        public int index { get; set; }
        public int value { get; set; }
    }
}
```
