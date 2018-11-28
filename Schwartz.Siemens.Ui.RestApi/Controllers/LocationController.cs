using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schwartz.Siemens.Core.ApplicationServices;
using Schwartz.Siemens.Core.Entities.Rigs;
using System.Collections.Generic;

namespace Schwartz.Siemens.Ui.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        public LocationController(ILocationService locationService)
        {
            LocationService = locationService;
        }

        private ILocationService LocationService { get; }

        [HttpGet]
        public ActionResult<List<Location>> GetAllLocations()
        {
            return Ok(LocationService.ReadAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Location> GetLocationById(int id)
        {
            return Ok(LocationService.Read(id));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult<Location> CreateLocation([FromBody] Location location)
        {
            return Ok(LocationService.Create(location));
        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<Location> UpdateLocation(int id, [FromBody] Location location)
        {
            return Ok(LocationService.Update(id, location));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult<Location> DeleteLocation(int id)
        {
            return Ok(LocationService.Delete(id));
        }
    }
}