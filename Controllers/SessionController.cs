using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace _001TN0172.Controllers
{
    [ApiController]
    [Route("session")]
    public class SessionController : ControllerBase
    {
        [HttpPost("ping")]
        public IActionResult Ping()
        {
            // Optional: update last seen timestamp in DB
            return Ok();
        }
    }

}
