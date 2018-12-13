using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schwartz.Siemens.Core.ApplicationServices;
using Schwartz.Siemens.Core.Entities.Rigs;
using System;

namespace Schwartz.Siemens.Ui.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RigsController : ControllerBase
    {
        public RigsController(IRigService rigService)
        {
            RigService = rigService;
        }

        private IRigService RigService { get; }

        [HttpGet]
        public ActionResult<FilteredList<Rig>> GetAllRigs(int page = 0, int items = 0)
        {
            return Ok(RigService.ReadAll(page, items));
        }

        [HttpGet("{id}")]
        public ActionResult<Rig> GetRig(int id)
        {
            return Ok(RigService.Read(id));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult<Rig> CreateRig([FromBody] Rig rig)
        {
            try
            {
                return Ok(RigService.Create(rig));
            }
            catch (ArgumentOutOfRangeException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut("{imo}")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<Rig> UpdateRig(int imo, [FromBody] Rig rig)
        {
            try
            {
                var update = RigService.Update(imo, rig);
                if (update == null)
                {
                    return BadRequest();
                }

                return Ok(update);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<Rig> DeleteRig(int id)
        {
            return Ok(RigService.Delete(id));
        }
    }
}