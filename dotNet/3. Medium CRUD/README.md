# Medium CRUD
* Create DTO
* Create controller and endpoints

# Create DTO
Create DTO by typing in terminal:
```sh
mkdir DTO
touch DTO/Body.cs
```
and then paste code below
```cs
using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class Body
    {
        [Required]
        public int index { get; set; }
        [Required]
        public int value { get; set; }
    }
}
```
# Create controller and endpoints
type in your terminal
```sh
touch Controllers/MediumController.cs
```
and paste code below
```cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.DTO;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MediumController : ControllerBase
    {
        private readonly ILogger _logger;
        private static List<int> _data { get; set; }
        public MediumController(ILogger<MediumController> logger)
        {
            _logger = logger;
            _data = new List<int>() { 1, 2, 3, 4, 5 };
        }

        [HttpPatch]
        public ActionResult patch([FromBody] Body body)
        {
            var s = Request.Headers["Content-Type"];
            _logger.LogInformation("idx: " + body.index + " value: " + body.value + ", s: " + s);
            _data.Insert(body.index, body.value);
            return Ok(new { message = "simple patch done", _data });
        }
    }
}
```
now you could check results in postman
