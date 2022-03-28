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
    [Route("api/[controller]")]
    [ApiController]
    public class NecklaceController : ControllerBase
    {
        private INecklaceRepository _repo;

        //GET: api/necklace
        //Below are good practice decorators to use for a GET request
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Necklace>))]
        public async Task<IEnumerable<Necklace>> GetNecklaces()
        {
            var necklace = await _repo.ReadAllAsync();
            return necklace;
        }

        //GET: api/customers/id
        [HttpGet("{custId}", Name = nameof(GetCustomer))]
        [ProducesResponseType(200, Type = typeof(Necklace))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCustomer(string custId)
        {
            if (!Guid.TryParse(custId, out Guid custGuid))
            {
                return BadRequest("Guid format error");
            }
            Necklace cust = await _repo.ReadAsync(custGuid);
            if (cust != null)
            {
                //cust is returned in the body
                return Ok(cust);
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
        [HttpDelete("{custId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCustomer(string custId)
        {
            if (!Guid.TryParse(custId, out Guid custGuid))
            {
                return BadRequest("Guid format error");
            }

            Necklace cust = await _repo.ReadAsync(custGuid);
            if (cust == null)
            {
                return NotFound();
            }

            cust = await _repo.DeleteAsync(custGuid);
            if (cust != null)
            {
                //Send an empty body response to confirm
                return new NoContentResult();
            }
            else
            {
                return BadRequest("Customer found but could not be deleted");
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
            AppLog.Instance.LogInformation("CustomersController started");
        }
    }
}
