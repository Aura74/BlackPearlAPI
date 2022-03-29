using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NecklaceCRUD;
using NecklaceDB;
using NecklaceModels;

namespace SeidoDbWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NecklaceController : ControllerBase
    {
        private INecklaceRepository _repo;

        //GET: api/necklace
        //Below are good practice decorators to use for a GET request
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Necklace>))]
        public async Task<IEnumerable<Necklace>> GetNecklaces()
        {
            var necklaces = await _repo.ReadAllAsync();
            return necklaces;
        }

        //GET: api/necklace/id
        [HttpGet("{neckId}", Name = nameof(GetNecklace))]
        [ProducesResponseType(200, Type = typeof(Necklace))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetNecklace(string neckId)
        {
            if (!int.TryParse(neckId, out int necklaceId))
            {
                return BadRequest("Int format error");
            }
            Necklace neck = await _repo.ReadAsync(necklaceId);
            if (neck != null)
            {
                //cust is returned in the body
                return Ok(neck);
            }
            else
            {
                return NotFound();
            }
        }

        //PUT: api/necklace/id
        //Body: Necklace in Json
        [HttpPut("{custId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCustomer(string custId, [FromBody] Necklace cust)
        {
            if (!Guid.TryParse(custId, out Guid custGuid))
            {
                return BadRequest("Guid format error");
            }
            if (custGuid != cust.CustomerID)
            {
                return BadRequest("Necklace ID mismatch");
            }

            cust = await _repo.UpdateAsync(cust);
            if (cust != null)
            {
                //Send an empty body response to confirm
                return new NoContentResult();
            }
            else
            {
                return NotFound();
            }
        }

        //DELETE: api/necklace/id
        [HttpDelete("{necklaceId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteNecklace(string necklaceId)
        {
            if (!int.TryParse(necklaceId, out int nId))
            {
                return BadRequest("int format error");
            }

            Necklace neck = await _repo.ReadAsync(nId);
            if (neck == null)
            {
                return NotFound();
            }

            neck = await _repo.DeleteAsync(nId);
            if (neck != null)
            {
                //Send an empty body response to confirm
                return new NoContentResult();
            }
            else
            {
                return BadRequest("Necklace found but could not be deleted");
            }
        }

        //POST: api/necklace
        //Body: Necklace in Json
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateCustomer([FromBody] Necklace cust)
        {
            if (cust == null)
            {
                return BadRequest("No Necklace");
            }
            if (await _repo.ReadAsync(cust.CustomerID) != null)
            {
                return BadRequest("Necklace ID already existing");
            }

            cust = await _repo.CreateAsync(cust);
            if (cust != null)
            {
                //201 created ok with url details to read newlys created Necklace
                return CreatedAtRoute(

                    //Named Route in the HttpGet request
                    routeName: nameof(GetCustomer),

                    //custId is the parameter in HttpGet
                    routeValues: new { custId = cust.CustomerID.ToString().ToLower() },

                    //Customer detail in the Body
                    value: cust);
            }
            else
            {
                return BadRequest("Could not create Necklace");
            }
        }


        public NecklaceController(INecklaceRepository repo, ILogger<NecklaceController> logger)
        {
            _repo = repo;
            logger.LogInformation("NecklaceController started");
            //AppLog.Instance.LogInformation("NecklaceController started");
        }
    }
}
