using Microsoft.AspNetCore.Mvc;
using Course_platform.Models.DTO;
using Course_platform.Models;
using System.Net.Mail;
using System.Net;

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

        [HttpPost("request_password_reset"),
           Produces("application/json"),
           Consumes("application/json")]
        public async Task<ActionResult> RequestPasswordReset([FromBody] EmailDTO sendEmail)
        {
            if (string.IsNullOrEmpty(sendEmail.Email) || !IsValidEmail(sendEmail.Email))
            {
                return BadRequest(new { error = "Неверный формат электронной почты." });
            }
            try
            {
                string token = GenerateCode();
                PasswordResetTokens passResetToken = new PasswordResetTokens
                {
                    Email = sendEmail.Email,
                    Token = token
                };

                _context.PasswordResetTokens.Add(passResetToken);
                _context.SaveChanges();

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

                    client.Send(message);
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ConfirmPassword([FromBody] PasswordRecoveryDTO passRecovery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (passRecovery.NewPassword != passRecovery.RepeatPassword)
            {
                ModelState.AddModelError("RepeatPassword", "Пароли не совпадают.");
                return BadRequest(ModelState);
            }

            User user = GetUserByEmail(passRecovery.Email, passRecovery.Token);
            if (user == null)
            {
                return NotFound(new { error = "Пользователь не найден или неверный код, попробуйте заново или вернитесь к генерации кода подтверждения" });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passRecovery.NewPassword);
            user.UpdatedAt = DateTime.Now;

            _context.Users.Update(user);
            _context.SaveChanges();

            PasswordResetTokens token = _context.PasswordResetTokens.FirstOrDefault(resetToken => resetToken.Email == passRecovery.Email && resetToken.Token == passRecovery.Token);
            if (token != null)
            {
                _context.PasswordResetTokens.Remove(token);
                _context.SaveChanges();
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

        public User GetUserByEmail(string email, string token)
        {
            PasswordResetTokens passwordResetToken = _context.PasswordResetTokens.FirstOrDefault(t => t.Email == email && t.Token == token);

            if (passwordResetToken != null)
            {
                DateTime createdAtUtc = passwordResetToken.CreatedAt.ToUniversalTime().Date;
                return _context.Users.FirstOrDefault(u => u.Email == email);
            }
            return null;
        }
    }
}
