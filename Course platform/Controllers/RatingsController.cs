using Course_platform.Models;
using Course_platform.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Course_platform.Controllers
{
    [Route("api/rating")]
    [ApiController]
    public class RatingsController : Controller
    {
        private readonly CoursePlatformDbContext _dbContext;

        public RatingsController(CoursePlatformDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // POST: api/ratings
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddOrUpdateRating([FromBody] RatingDTO ratingDto)
        {
            // Получение ID текущего пользователя
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = int.Parse(userIdClaim.Value);

            // Проверка, существует ли курс
            var course = _dbContext.Courses.FirstOrDefault(c => c.CourseId == ratingDto.CourseId);
            if (course == null)
            {
                return NotFound(new { message = "Курс не найден." });
            }

            // Проверка на существующий рейтинг
            var existingRating = _dbContext.Ratings.FirstOrDefault(r =>
                r.CourseId == ratingDto.CourseId && r.UserId == userId);

            if (existingRating != null)
            {
                // Обновление существующего рейтинга
                existingRating.UserRating = ratingDto.UserRating;
                await _dbContext.SaveChangesAsync();
                return Ok(new
                {
                    courseId = existingRating.CourseId,
                    userRating = existingRating.UserRating
                   
                });
            }

            // Добавление нового рейтинга
            var newRating = new RatingEntity
            {
                UserId = userId,
                CourseId = ratingDto.CourseId,
                UserRating = ratingDto.UserRating
              
            };

            _dbContext.Ratings.Add(newRating);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                courseId = newRating.CourseId,
                userRating = newRating.UserRating
              
            });
        }


        [Authorize]
        [HttpGet("qwerty/{courseId}")]
        public IActionResult GetCourseRatings(int courseId)
        {
            // Получение ID текущего пользователя
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = int.Parse(userIdClaim.Value);

            // Выборка всех оценок для указанного курса
            var ratings = _dbContext.Ratings.Where(r => r.CourseId == courseId);

            // Средняя оценка по курсу
            var averageRating = ratings.Any() ? ratings.Average(r => r.UserRating) : 0;

            // Оценка текущего пользователя
            var userRating = ratings.FirstOrDefault(r => r.UserId == userId)?.UserRating;

            // Возвращаем среднюю оценку и пользовательскую оценку
            return Ok(new
            {
                averageRating,
                userRating = userRating ?? 0 // Если пользователь ещё не выставил оценку, возвращаем 0
            });
        }

        // GET: api/courses/{courseId}/ratings
        [HttpGet("{courseId}/ratings")]
        [AllowAnonymous] // Если хотите разрешить доступ без авторизации
        public async Task<IActionResult> GetAllRatingsForCourse(int courseId)
        {
            // Проверка, существует ли курс
            var course = await _dbContext.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null)
            {
                return NotFound(new { message = "Курс не найден." });
            }

            // Извлечение всех рейтингов для курса
            var ratings = await _dbContext.Ratings
                                          .Where(r => r.CourseId == courseId)
                                          .Select(r => new
                                          {
                                              userId = r.UserId,
                                              value = r.UserRating
                                          })
                                          .ToListAsync();

            // Если рейтингов нет, возвращаем пустой массив
            if (!ratings.Any())
            {
                return Ok(new List<object>());
            }

            // Возвращаем все рейтинги
            return Ok(ratings);
        }

    }
}