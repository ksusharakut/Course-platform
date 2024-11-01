using Course_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course_platform.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly CoursePlatformDbContext _context;

        public UserController(CoursePlatformDbContext context)
        {
            _context = context;
        }

        [HttpGet,
            Produces("application/json"),
            Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetUsers()
        {
            return Ok();
        }

        [HttpGet,
           Produces("application/json"),
           Consumes("application/json")]
        public async Task<ActionResult<UserEntity>> GetUserById(int id)
        {
            return Ok();
        }

        [Authorize]
        [HttpGet,
           Produces("application/json"),
           Consumes("application/json")]
        public async Task<ActionResult<UserEntity>> GetProfile()
        {
            return Ok();
        }

        [HttpPost,
           Produces("application/json"),
           Consumes("application/json")]
        public async Task<ActionResult<UserEntity>> CreateUser([FromBody] UserEntity user)
        {
            return Ok();
        }

        [HttpPatch,
            Produces("application/json"),
            Consumes("application/json")]
        public async Task<ActionResult<UserEntity>> UpdateUser([FromBody] UserEntity user)
        {
            return Ok();
        }

        [HttpDelete,
            Produces("application/json"),
            Consumes("application/json")]
        public async Task<ActionResult<UserEntity>> DeleteUser(int id)
        {
            return BadRequest();
        }
    }

}
