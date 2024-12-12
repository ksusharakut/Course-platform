using Microsoft.AspNetCore.Mvc;
using Course_platform.Models.DTO;
using Course_platform.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace Course_platform.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly CoursePlatformDbContext _context;

        public AuthController(CoursePlatformDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("login"),
            Produces("application/json"),
            Consumes("application/json")]
        public async Task<IActionResult> Login([FromBody] LogInDTO loginModel)
        {
            UserEntity user = await Authenticate(loginModel);

            if (user != null)
            {
                object response = Generate(user);
                return Ok(response);
            }

           
            else
            {
                Console.WriteLine($"Не удалось найти пользователя с email: {loginModel.Email}");
                return NotFound("User not found");
            }
        }

        private object Generate(UserEntity user)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("id", user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                Environment.GetEnvironmentVariable("JWT_ISSUER"),
                Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new
            {
                Token = tokenString,
                User = new
                {
                    user.UserId,
                    user.Nickname,
                    user.Email,
                    user.UpdatedAt,
                    user.DateBirth,
                    user.Role,
                    user.AccountBalance
                }
            };
        }

        [HttpPost("register"),
           Produces("application/json"),
           Consumes("application/json")]
        public async Task<ActionResult<UserEntity>> RegistrateUser([FromBody] RegistrateUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(await _context.Users.AnyAsync(existingUser => existingUser.Email.ToLower() == userDTO.Email.ToLower()))
            {
                ModelState.AddModelError("CustomError", "User already exists!");
                return BadRequest(ModelState);
            }

            if (userDTO == null)
            {
                return BadRequest("User cannot be null");
            }

            UserEntity user = new UserEntity
            {
                Nickname = userDTO.Nickname,
                Email = userDTO.Email,
                DateBirth = userDTO.DateBirth,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.Password)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            ReturnUserDTO returnUserDTO = new ReturnUserDTO
            {
                UserId = user.UserId,
                Nickname = user.Nickname,
                DateBirth = user.DateBirth,
                Email = user.Email,
                Role = user.Role,
                AccountBalance = user.AccountBalance
            };

            return Ok(returnUserDTO);
        }

        [HttpPost("request_password_reset"),
           Produces("application/json"),
           Consumes("application/json")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] EmailDTO sendEmail)
        {
            if (string.IsNullOrEmpty(sendEmail.Email) || !IsValidEmail(sendEmail.Email))
            {
                return BadRequest(new { error = "Неверный формат электронной почты." });
            }
            try
            {

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == sendEmail.Email);
                if (user == null)
                {
                    return BadRequest(new { error = "Аккаунт с таким email не найден." });
                }

                string token = GenerateCode();
                PasswordResetTokenEntity passResetToken = new PasswordResetTokenEntity
                {
                    Email = sendEmail.Email,
                    Token = token
                };

                await _context.PasswordResetTokens.AddAsync(passResetToken);
                await _context.SaveChangesAsync();

                string smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER");
                string port = Environment.GetEnvironmentVariable("SMTP_PORT");

                string username = Environment.GetEnvironmentVariable("USERNAME");
                string password = Environment.GetEnvironmentVariable("PASSWORD");

                using (SmtpClient client = new SmtpClient(smtpServer, Convert.ToInt16(port)))
                {
                    client.Credentials = new NetworkCredential(username, password);
                    client.EnableSsl = false;

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(Environment.GetEnvironmentVariable("FROM_EMAIL"));
                    message.To.Add(passResetToken.Email);
                    message.Subject = "Код восстановления пароля";
                    message.Body = $"Ваш код восстановления: {token}";

                    await client.SendMailAsync(message);
                    return Ok(new { message = "Письмо с кодом отправлено." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке письма: {ex.Message}");
                return BadRequest(new { error = "Не удалось отправить письмо." });
            }
        }

        [HttpPost("reset_password")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task <IActionResult> ConfirmPassword([FromBody] PasswordRecoveryDTO passRecovery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserEntity user = await GetUserByEmail(passRecovery.Email, passRecovery.Token);
            if (user == null)
            {
                return NotFound(new { error = "Пользователь не найден или неверный код, попробуйте заново или вернитесь к генерации кода подтверждения" });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passRecovery.NewPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            PasswordResetTokenEntity token = await _context.PasswordResetTokens.FirstOrDefaultAsync(resetToken => resetToken.Email == passRecovery.Email && resetToken.Token == passRecovery.Token);
            if (token != null)
            {
                _context.PasswordResetTokens.Remove(token);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Пароль успешно обновлен." });
        }

        private string GenerateCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async Task<UserEntity> GetUserByEmail(string email, string token)
        {
            PasswordResetTokenEntity passwordResetToken = await _context.PasswordResetTokens.FirstOrDefaultAsync(t => t.Email == email && t.Token == token);

            if (passwordResetToken != null)
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            return null;
        }
        private async Task<UserEntity> Authenticate(LogInDTO loginModel)
        {
            UserEntity currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() ==
               loginModel.Email.ToLower());
            if (currentUser != null && BCrypt.Net.BCrypt.Verify(loginModel.Password, currentUser.PasswordHash))
            {
                return currentUser;
            }
            return null;
        }
    }
}
