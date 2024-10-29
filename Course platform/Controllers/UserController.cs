using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course_platform.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly NovaMindContext _context;

        public UserController(NovaMindContext context)
        {
            _context = context;
        }

        [HttpGet,
            Produces("application/json"),
            Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok();
        }

        [HttpGet,
           Produces("application/json"),
           Consumes("application/json")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            return Ok();
        }

        [Authorize]
        [HttpGet,
           Produces("application/json"),
           Consumes("application/json")]
        public async Task<ActionResult<User>> GetProfile()
        {
            return Ok();
        }

        [HttpPost,
           Produces("application/json"),
           Consumes("application/json")]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            return Ok();
        }

        [HttpPatch,
            Produces("application/json"),
            Consumes("application/json")]
        public async Task<ActionResult<User>> UpdateUser([FromBody] User user)
        {
            return Ok();
        }

        [HttpDelete,
            Produces("application/json"),
            Consumes("application/json")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            return BadRequest();
        }
    }

}
