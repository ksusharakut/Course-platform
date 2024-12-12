using Course_platform.Models;
using Course_platform.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Course_platform.Controllers
{
    [ApiController]
    [Route("api/course")]
    public class CourseController : ControllerBase
    {
        private readonly CoursePlatformDbContext _context;

        public CourseController(CoursePlatformDbContext context)
        {
            _context = context;
        }


        [Authorize]
        [HttpGet("all-sales")]
        public async Task<IActionResult> GetCourseSales()
        {
            var salesData = await _context.Transactions
                .Where(t => t.TransactionType == "Purchase")
                .GroupBy(t => t.CourseId)
                .Select(group => new
                {
                    CourseId = group.Key,
                    SalesCount = group.Count()
                })
                .ToListAsync();

            return Ok(salesData);
        }

        [Authorize]
        [HttpGet("user/courses")]
        public async Task<IActionResult> GetUserCourses()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = int.Parse(userIdClaim.Value);

            var courses = await _context.Courses
                .Where(c => c.UserId == userId) 
                .Include(c => c.Categories) 
                .ToListAsync();

            if (!courses.Any())
            {
                return NotFound("No courses found for this user.");
            }

            var courseDtos = courses.Select(course => new ReturnCourseDTO
            {
                CourseId = course.CourseId,
                UserId = course.UserId,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                IsPublished = course.IsPublished,
                CategoryIds = course.Categories.Select(c => c.CategoryId).ToList()
            }).ToList();

            return Ok(courseDtos); 
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseDTO courseDTO)
        {
            if (courseDTO == null)
            {
                return BadRequest("Invalid course data.");
            }

            // Получаем UserId из токена
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = int.Parse(userIdClaim.Value); // Преобразуем UserId в нужный тип (например, int)

            // Создаём новый курс
            var course = new CourseEntity
            {
                UserId = userId,  // Присваиваем UserId из токена
                Title = courseDTO.Title,
                Description = courseDTO.Description,
                Price = courseDTO.Price,
                CreatedAt = DateTimeOffset.UtcNow
            };

            // Получаем категории из базы данных по переданным ID
            var categories = await _context.Categories
                .Where(c => courseDTO.CategoryIds.Contains(c.CategoryId))
                .ToListAsync();

            // Проверяем, что все переданные ID категорий были найдены в базе данных
            if (categories.Count != courseDTO.CategoryIds.Count)
            {
                return BadRequest("Some categories are invalid.");
            }

            // Привязываем категории к курсу
            course.Categories = categories;

            // Сохраняем курс в базе данных
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourseById), new { id = course.CourseId }, course);
        }

        [Authorize]
        [HttpGet("{id}")]
            public async Task<IActionResult> GetCourseById(int id)
            {
                var course = await _context.Courses.FindAsync(id);

                if (course == null)
                {
                    return NotFound("Курс не найден.");
                }

                return Ok(course);
            }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Categories)  // Загружаем связанные категории
                .ToListAsync();

            var courseDtos = courses.Select(course => new ReturnCourseDTO
            {
                CourseId = course.CourseId,
                UserId = course.UserId,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                IsPublished = course.IsPublished, // Проверьте, что это значение корректно
                CategoryIds = course.Categories.Select(c => c.CategoryId).ToList()
            }).ToList();

            return Ok(courseDtos);
        }

        [Authorize]
        [HttpGet("publishedcourses")]
        public async Task<IActionResult> GetPublishedCourses()
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = int.Parse(userIdClaim.Value); // Преобразуем UserId в нужный тип (например, int)
            // Получаем ID текущего пользователя
          
            // Загружаем опубликованные курсы, созданные не текущим пользователем
            var courses = await _context.Courses
                .Where(c => c.IsPublished && c.UserId != userId) // Исключаем курсы текущего пользователя
                .Include(c => c.Categories) // Подгружаем категории
                .ToListAsync();

            // Преобразуем курсы в DTO
            var courseDtos = courses.Select(course => new ReturnCourseDTO
            {
                CourseId = course.CourseId,
                UserId = course.UserId,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                CategoryIds = course.Categories.Select(c => c.CategoryId).ToList()
            }).ToList();

            return Ok(courseDtos);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDTO courseDTO)
        {
            if (courseDTO == null)
            {
                return BadRequest("Invalid course data.");
            }

            // Получаем UserId из токена
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = int.Parse(userIdClaim.Value);

            // Находим курс, который необходимо обновить
            var course = await _context.Courses
                .Include(c => c.Categories) // Загружаем связанные категории
                .FirstOrDefaultAsync(c => c.CourseId == id && c.UserId == userId);

            if (course == null)
            {
                return NotFound("Курс не найден или не принадлежит текущему пользователю.");
            }

            // Обновляем данные курса
            course.Title = courseDTO.Title;
            course.Description = courseDTO.Description;
            course.Price = courseDTO.Price;

            // Получаем категории из базы данных по переданным ID
            var categories = await _context.Categories
                .Where(c => courseDTO.CategoryIds.Contains(c.CategoryId))
                .ToListAsync();

            // Проверяем, что все переданные ID категорий были найдены в базе данных
            if (categories.Count != courseDTO.CategoryIds.Count)
            {
                return BadRequest("Some categories are invalid.");
            }

            // Обновляем привязку категорий
            course.Categories.Clear(); // Очищаем текущие категории
            course.Categories = categories; // Добавляем новые категории

            // Сохраняем изменения
            await _context.SaveChangesAsync();

            // Возвращаем обновлённый курс
            return Ok(new ReturnCourseDTO
            {
                CourseId = course.CourseId,
                UserId = course.UserId,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                CategoryIds = course.Categories.Select(c => c.CategoryId).ToList()
            });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            // Получаем UserId из токена
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = int.Parse(userIdClaim.Value); // Преобразуем UserId в нужный тип (например, int)

            // Находим курс по ID
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == id && c.UserId == userId);

            if (course == null)
            {
                return NotFound("Курс не найден или не принадлежит текущему пользователю.");
            }

            // Удаляем курс
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();  // Возвращаем статус 204 (No Content) после успешного удаления
        }

        [HttpPost("{courseId}/set-publish-status")]
        public async Task<IActionResult> SetPublishStatus(int courseId, [FromBody] PublishStatusRequestDTO request)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound("Курс не найден.");

            if (course.IsPublished == request.IsPublished)
                return BadRequest(request.IsPublished
                    ? "Курс уже опубликован."
                    : "Курс уже скрыт.");

            course.IsPublished = request.IsPublished;
            await _context.SaveChangesAsync();

            // Возвращаем обновлённый объект
            return Ok(new ReturnCourseDTO
            {
                CourseId = course.CourseId,
                UserId = course.UserId,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                IsPublished = course.IsPublished,
                CategoryIds = course.Categories.Select(c => c.CategoryId).ToList()
            });
        }

        [Authorize]
        [HttpGet("published/{userId}")]
        public async Task<IActionResult> GetPublishedCoursesByUser(int userId)
        {
            // Фильтрация курсов по UserId и IsPublished
            var courses = await _context.Courses
                .Where(c => c.UserId == userId && c.IsPublished) // Только опубликованные курсы пользователя
                .Include(c => c.Categories) // Загружаем категории
                .ToListAsync();

            if (!courses.Any())
            {
                return NotFound("Нет опубликованных курсов для данного пользователя.");
            }

            // Преобразуем курсы в DTO
            var courseDtos = courses.Select(course => new ReturnCourseDTO
            {
                CourseId = course.CourseId,
                UserId = course.UserId,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                IsPublished = course.IsPublished,
                CategoryIds = course.Categories.Select(c => c.CategoryId).ToList()
            }).ToList();

            return Ok(courseDtos); // Возвращаем список опубликованных курсов
        }

    }

}

