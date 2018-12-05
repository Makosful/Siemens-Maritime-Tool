using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schwartz.Siemens.Core.ApplicationServices;
using Schwartz.Siemens.Core.Entities.Rigs;
using System.Collections.Generic;

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
        public ActionResult<List<Rig>> GetAllRigs()
        {
            return Ok(RigService.ReadAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Rig> GetRig(int id)
        {
            return Ok(RigService.Read(id));
        }

        //[HttpPost("update")]
        public ActionResult M([FromBody] List<int> ids)
        {
            return Ok(RigService.UpdateLocations(ids));
        }

        //[HttpPost]
        //[Authorize(Roles = "Administrator")]
        public ActionResult<Rig> CreateRig([FromBody] Rig rig)
        {
            return Ok(RigService.Create(rig));
        }

        //[HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<Rig> UpdateRig(int id, [FromBody] Rig rig)
        {
            return Ok(RigService.Update(id, rig));
        }

        //[HttpDelete("{id}")]
        //[Authorize(Roles = "Administrator")]
        public ActionResult<Rig> DeleteRig(int id)
        {
            return Ok(RigService.Delete(id));
        }
    }
}