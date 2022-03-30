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
        private ILogger _logger;

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
            var neck = await _repo.ReadAsync(necklaceId);
            if (neck != null)
            {
                return Ok(neck);
            }
            else
            {
                return NotFound();
            }
        }

        //PUT: api/necklace/id
        //Body: Necklace in Json
        [HttpPut("{neckId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateNecklace(string neckId, [FromBody] Necklace neck)
        {
            if (!int.TryParse(neckId, out int necklaceId))
            {
                return BadRequest("Int format error");
            }
            if (necklaceId != neck.NecklaceID)
            {
                return BadRequest("Necklace ID mismatch");
            }

            neck = await _repo.UpdateAsync(neck);
            if (neck != null)
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
        public async Task<IActionResult> CreateNecklace([FromBody] Necklace neck)
        {
            if (neck == null)
            {
                return BadRequest("No Necklace");
            }

            var newNecklace = new Necklace();

            foreach(var p in neck.Pearls)
            {
                var newPearl = new Pearl();
                newPearl.Size = p.Size;
                newPearl.Color = p.Color;
                newPearl.Shape = p.Shape;
                newPearl.Type = p.Type;

                newNecklace.Pearls.Add(newPearl);
            }

            neck = await _repo.CreateAsync(newNecklace);
            if (neck != null)
            {
                //201 created ok with url details to read newly created Necklace
                return CreatedAtRoute(

                    //Named Route in the HttpGet request
                    routeName: nameof(GetNecklace),

                    //custId is the parameter in HttpGet
                    routeValues: new { neckId = neck.NecklaceID.ToString().ToLower() },

                    //Customer detail in the Body
                    value: neck);
            }
            else
            {
                return BadRequest("Could not create Necklace");
            }
        }


        public NecklaceController(INecklaceRepository repo, ILogger<NecklaceController> logger)
        {
            _repo = repo;
            _logger = logger;
            _logger.LogInformation("NecklaceController started");
        }
    }
}
