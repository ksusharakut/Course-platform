using Course_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Course_platform.Controllers
{
    [ApiController]
    [Route("api/usercourseprogress")]
    public class UserCourseProgressController : ControllerBase
    {
        private readonly CoursePlatformDbContext _context;

        public UserCourseProgressController(CoursePlatformDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("me/courses")]
        public async Task<IActionResult> GetUserCourses()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = int.Parse(userIdClaim.Value);

            // Получаем курсы пользователя
            var courses = await _context.UserCourseProgresses
                .Where(u => u.UserId == userId)
                .Include(u => u.Course)
                .Select(u => new
                {
                    u.CourseId,
                    Title = u.Course.Title,
                    Description = u.Course.Description,
                    Price = u.Course.Price,
                    u.State
                })
                .ToListAsync();

            if (!courses.Any())
                return NotFound("У пользователя нет приобретённых курсов.");

            return Ok(courses);
        }


        [Authorize]
        [HttpPost("purchase/{courseId}")]
        public async Task<IActionResult> PurchaseCourse(int courseId)
        {
            try
            {
                var buyerId = GetCurrentUserId(); // Получаем текущего пользователя (покупателя)

                // Получаем данные курса
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
                if (course == null)
                {
                    return NotFound(new { Message = "Курс не найден." });
                }

                // Получаем продавца (создателя курса)
                var seller = await _context.Users.FirstOrDefaultAsync(u => u.UserId == course.UserId);
                if (seller == null)
                {
                    return Unauthorized(new { Message = "Продавец курса не найден." });
                }

                // Получаем покупателя
                var buyer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == buyerId);
                if (buyer == null)
                {
                    return Unauthorized(new { Message = "Покупатель не найден." });
                }

                // Проверяем, достаточно ли средств на балансе покупателя
                if (buyer.AccountBalance < course.Price)
                {
                    return BadRequest(new { Message = "Недостаточно средств для покупки курса." });
                }

                // Проверяем, не купил ли покупатель уже этот курс
                var existingProgress = await _context.UserCourseProgresses
                    .FirstOrDefaultAsync(p => p.UserId == buyerId && p.CourseId == courseId);

                if (existingProgress != null)
                {
                    return BadRequest(new { Message = "Курс уже был куплен ранее." });
                }

                // Выполнение финансовых операций
                buyer.AccountBalance -= course.Price;
                seller.AccountBalance += course.Price;

                // Создаем записи о транзакциях
                var buyerTransaction = new TransactionEntity
                {
                    UserId = buyerId,
                    Amount = -course.Price, // Отрицательное значение для списания
                    TransactionType = "Покупка курса",
                    CreatedAt = DateTimeOffset.UtcNow,
                    CourseId = courseId
                };

                var sellerTransaction = new TransactionEntity
                {
                    UserId = seller.UserId,
                    Amount = course.Price, // Положительное значение для зачисления
                    TransactionType = "Продажа курса",
                    CreatedAt = DateTimeOffset.UtcNow,
                    CourseId = courseId
                };

                // Создаем запись о прогрессе курса
                var progress = new UserCourseProgressEntity
                {
                    UserId = buyerId,
                    CourseId = courseId,
                    State = "Started",
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                // Добавляем транзакции, прогресс и обновляем пользователей
                _context.Transactions.Add(buyerTransaction);
                _context.Transactions.Add(sellerTransaction);
                _context.UserCourseProgresses.Add(progress);
                _context.Users.Update(buyer);
                _context.Users.Update(seller);

                // Сохраняем изменения
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Покупка курса успешна." });
            }
            catch (Exception ex)
            {
                // Логирование ошибки для анализа
                // _logger.LogError(ex, "Ошибка при покупке курса.");
                return StatusCode(500, new { Message = "Произошла ошибка при обработке запроса." });
            }
        }



        private int GetCurrentUserId()
        {
            // Получаем текущего пользователя из токена
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            // Если не удается извлечь ID, выбрасываем ошибку
            throw new UnauthorizedAccessException("Не удалось получить идентификатор пользователя.");
        }

        [HttpGet("check-progress/{courseId}")]
        public async Task<IActionResult> CheckCourseProgress(int courseId)
        {
            var userId = GetCurrentUserId(); // Получаем текущего пользователя из токена

            // Ищем запись о прогрессе пользователя для данного курса
            var progress = await _context.UserCourseProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId);

            // Если запись найдена, значит курс был начат
            if (progress != null && progress.State == "Started")
            {
                return Ok(new { started = true });
            }

            // Если записи нет или курс не начат
            return Ok(new { started = false });
        }
    }
}
