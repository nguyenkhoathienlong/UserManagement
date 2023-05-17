using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace UserManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleAssignmentController : ControllerBase
    {
        private readonly IRoleAssignmentService _roleAssignmentService;

        public RoleAssignmentController(IRoleAssignmentService roleAssignmentService)
        {
            _roleAssignmentService = roleAssignmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roleAssignmentService.GetAll();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleAssignmentCreateModel model)
        {
            var result = await _roleAssignmentService.Create(model);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RoleAssignmentUpdateModel model)
        {
            var result = await _roleAssignmentService.Update(id, model);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _roleAssignmentService.Delete(id);
            return Ok(result);
        }
    }
}
