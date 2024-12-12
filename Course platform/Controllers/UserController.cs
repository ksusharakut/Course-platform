using Course_platform.Models;
using Course_platform.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        [Authorize]
        [HttpGet,
            Produces("application/json"),
            Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetUsers()
        {
            var users = await _context.Users
            .Select(user => new ReturnUserDTO
            {
                UserId = user.UserId,
                Nickname = user.Nickname,
                DateBirth = user.DateBirth,
                Email = user.Email,
                Role = user.Role,
                AccountBalance = user.AccountBalance
            })
            .ToListAsync();

            return Ok(users);
        }

        [HttpGet("profile"),
           Produces("application/json"),
           Consumes("application/json")]
        public async Task<ActionResult<UserEntity>> GetUserProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = int.Parse(userIdClaim.Value); // Преобразуем UserId в нужный тип (например, int)

            var user = await _context.Users
            .Where(u => u.UserId == userId)
            .Select(u => new ReturnUserDTO
            {
                UserId = u.UserId,
                Nickname = u.Nickname,
                DateBirth = u.DateBirth,
                Email = u.Email,
                Role = u.Role,
                AccountBalance = u.AccountBalance
            })
            .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            return Ok(user);
        }

        //[Authorize]
        //[HttpGet,
        //   Produces("application/json"),
        //   Consumes("application/json")]
        //public async Task<ActionResult<UserEntity>> GetProfile()
        //{
        //    return Ok();
        //}

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



        [HttpGet("{userId}"),
    Produces("application/json"),
    Consumes("application/json")]
        public async Task<ActionResult<ReturnUserDTO>> GetUserById(int userId)
        {
            // Ищем пользователя по переданному ID
            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new ReturnUserDTO
                {
                    UserId = u.UserId,
                    Nickname = u.Nickname,
                    DateBirth = u.DateBirth,
                    Email = u.Email,
                    Role = u.Role,
                    AccountBalance = u.AccountBalance
                })
                .FirstOrDefaultAsync();

            // Если пользователь не найден
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            // Возвращаем данные пользователя
            return Ok(user);
        }


        [Authorize]
        [HttpPatch("{userId}/topup")]
        public async Task<IActionResult> TopUpBalance(int userId, [FromBody] TopUpRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Пользователь не найден." });
            }

            // Увеличиваем баланс пользователя
            user.AccountBalance += request.Amount;

            // Создаем транзакцию
            var transaction = new TransactionEntity
            {
                UserId = userId,
                Amount = request.Amount,
                TransactionType = "Пополнение баланса",
                CreatedAt = DateTimeOffset.UtcNow,

            };

            // Добавляем транзакцию в базу данных
            _context.Transactions.Add(transaction);

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            return Ok(new { success = true, accountBalance = user.AccountBalance });
        }

        [Authorize]
        [HttpPatch("{userId}/deduct")]
        public async Task<IActionResult> DeductBalance(int userId, [FromBody] DeductRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Пользователь не найден." });
            }

            // Проверяем, достаточно ли средств на балансе
            if (user.AccountBalance < request.Amount)
            {
                return BadRequest(new { success = false, message = "Недостаточно средств на балансе." });
            }

            // Уменьшаем баланс пользователя
            user.AccountBalance -= request.Amount;

            // Создаем транзакцию
            var transaction = new TransactionEntity
            {
                UserId = userId,
                Amount = request.Amount,
                TransactionType = "Вывод средств",
                CreatedAt = DateTimeOffset.UtcNow
            };

            // Добавляем транзакцию в базу данных
            _context.Transactions.Add(transaction);

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            return Ok(new { success = true, accountBalance = user.AccountBalance });
        }




        [HttpGet("{userId}/balance")]
        public async Task<ActionResult<decimal>> GetBalance(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            // Возвращаем только баланс пользователя
            return Ok(user.AccountBalance);
        }



        [HttpGet("{userId}/transactions")]
        public async Task<ActionResult<IEnumerable<TransactionEntity>>> GetTransactionsByUserId(int userId)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound(new { success = false, message = "Пользователь не найден." });
                }

                var transactions = await _context.Transactions
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                if (!transactions.Any())
                {
                    return NotFound(new { success = false, message = "Транзакции не найдены." });
                }

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем сообщение
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера." });
            }
        }



    }

}
