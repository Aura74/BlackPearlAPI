using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DbAppWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        public LogController() { }

        //GET /Log
        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AppLogItem>))]
        public IEnumerable<AppLogItem> Get()
        {
           return AppLog.Instance.ToArray();
        }
    }
}
