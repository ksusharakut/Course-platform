using Microsoft.AspNetCore.Mvc;
using Course_platform.Models.DTO;

namespace Course_platform.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly NovaMindContext _context;

        public AuthController(NovaMindContext context)
        {
            _context = context;
        }

        [HttpPost("register"),
           Produces("application/json"),
           Consumes("application/json")]
        public async Task<ActionResult<User>> RegistrateUser([FromBody] RegistrateUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(_context.Users.Any(existingUser => existingUser.Email.ToLower() == userDTO.Email.ToLower()))
            {
                ModelState.AddModelError("CustomError", "User already exists!");
                return BadRequest(ModelState);
            }

            if (userDTO == null)
            {
                return BadRequest("User cannot be null");
            }

            User user = new User
            {
                Nickname = userDTO.Nickname,
                Email = userDTO.Email,
                DateBirth = userDTO.DateBirth,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ReturnUserDTO returnUserDTO = new ReturnUserDTO
            {
                UserId = user.UserId,
                Nickname = user.Nickname,
                DateBirth = user.DateBirth,
                Email = user.Email,
                Role = user.Role,
                VerifiedDegree = user.VerifiedDegree,
                AccountBalance = user.AccountBalance
            };

            return Ok(returnUserDTO);
        }
    }
}
