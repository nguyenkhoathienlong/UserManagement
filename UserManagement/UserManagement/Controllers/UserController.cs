using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("/login")]
        public async Task<IActionResult>Login(UserRequest model)
        {
            var data = await _userService.Login(model);
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpPost("/register")]
        public async Task<IActionResult> Register(UserCreateModel model)
        {
            var data = await _userService.Register(model);
            return Ok(data);
        }

        [Authorize(Roles = "Adminstrator")]
        [HttpGet]
        public async Task<IActionResult> GetAllUser([FromQuery] UserQueryModel query)
        {
            var result = await _userService.GetAll(query);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _userService.GetById(id);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateModel model)
        {
            var result = await _userService.Create(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateModel model)
        {
            var result = await _userService.Update(id, model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.Delete(id);
            return Ok(result);
        }
    }
}
