using Course_platform.Models.DTO;
using Course_platform.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Course_platform.Controllers
{
    [ApiController]
    public class LessonController : Controller
    {
        private readonly CoursePlatformDbContext _context;
        private const string LessonsDirectory = @"D:\course project folder for lessons\";

        public LessonController(CoursePlatformDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("api/course/{courseId}/units/{unitId}/lessons/reorder")]
        public async Task<IActionResult> ReorderLessons(int courseId, int unitId, [FromBody] List<ReorderLessonDTO> reorderLessonDtos)
        {
            var unit = await _context.Units.FindAsync(unitId);
            if (unit == null)
            {
                return NotFound(new { Message = "Unit not found" });
            }

            if (reorderLessonDtos == null || reorderLessonDtos.Count == 0)
            {
                return BadRequest(new { Message = "The list of lessons to reorder cannot be empty." });
            }

            var lessons = await _context.Lessons
                .Where(l => l.UnitId == unitId)
                .ToListAsync();


            foreach (var reorderLessonDto in reorderLessonDtos)
            {
                var lesson = lessons.FirstOrDefault(l => l.LessonId == reorderLessonDto.LessonId);
                if (lesson != null)
                {
                    lesson.OrderIndex = reorderLessonDto.NewOrderIndex;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Lessons reordered successfully" });
        }


        [Authorize]
        [HttpPost]
        [Route("api/course/{courseId}/createunits/{unitId}/createlessons")]
        public async Task<IActionResult> CreateLesson(int courseId, int unitId, [FromBody] CreateLessonDTO lessonDto)
        {
            var unit = await _context.Units.FindAsync(unitId);
            if (unit == null)
            {
                return NotFound(new { Message = "Unit not found" });
            }

            var sanitizedTitle = string.Concat(lessonDto.Title.Where(char.IsLetterOrDigit));
            var fileName = $"{sanitizedTitle}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.html";
            var directoryPath = @"D:\course_project_folder_for_lessons";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, fileName);

            try
            {
                await System.IO.File.WriteAllTextAsync(filePath, lessonDto.Content);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to save the file", Details = ex.Message });
            }

            var maxOrderIndex = await _context.Lessons
                .Where(l => l.UnitId == unitId)
                .MaxAsync(l => (int?)l.OrderIndex) ?? 0;

            var lesson = new LessonEntity
            {
                UnitId = unitId,
                Title = lessonDto.Title,
                FilePath = filePath,
                OrderIndex = maxOrderIndex + 1 
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLessonById), new { id = lesson.LessonId }, lesson);
        }


        [Authorize]
        [HttpPut("api/course/{courseId}/units/{unitId}/lessons/{lessonId}")]
        public async Task<IActionResult> UpdateLesson(int lessonId, [FromBody] UpdateLessonDTO lessonDto)
        {
            var lesson = await _context.Lessons.FindAsync(lessonId);
            if (lesson == null)
            {
                return NotFound(new { Message = "Lesson not found" });
            }

            try
            {
                await System.IO.File.WriteAllTextAsync(lesson.FilePath, lessonDto.Content);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to update the file", Details = ex.Message });
            }

            if (!string.IsNullOrEmpty(lessonDto.Title))
            {
                lesson.Title = lessonDto.Title;
            }

            if (lessonDto.OrderIndex != null)
            {
                lesson.OrderIndex = lessonDto.OrderIndex;
            }

            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Lesson updated successfully" });
        }



        [Authorize]
        [HttpGet("api/course/{courseId}/units/{unitId}/lessons")]
        public async Task<IActionResult> GetLessonsByUnitId(int unitId)
        {
            var lessons = await _context.Lessons
                .Where(lesson => lesson.UnitId == unitId)
                .OrderBy(lesson => lesson.OrderIndex)
                .Select(lesson => new
                {
                    lesson.LessonId,
                    lesson.Title,
                    lesson.FilePath,
                    lesson.OrderIndex
                })
                .ToListAsync();

            return Ok(lessons);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLessonById(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound(new { Message = "Lesson not found" });
            }

            return Ok(lesson);
        }

        [Authorize]
        [HttpGet("api/course/{courseId}/units/{unitId}/lessons/{lessonId}/content")]
        public async Task<IActionResult> GetLessonContent(int lessonId)
        {
            var lesson = await _context.Lessons.FindAsync(lessonId);
            if (lesson == null)
            {
                return NotFound(new { Message = "Lesson not found" });
            }

            if (!System.IO.File.Exists(lesson.FilePath))
            {
                return NotFound(new { Message = "Lesson content file not found" });
            }

            try
            {
                var content = await System.IO.File.ReadAllTextAsync(lesson.FilePath);
                return Ok(new { Content = content });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to read the lesson content", Details = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("api/course/{courseId}/units/{unitId}/lessons/{lessonId}")]
        public async Task<IActionResult> DeleteLesson(int courseId, int unitId, int lessonId)
        {
            var lesson = await _context.Lessons.FindAsync(lessonId);
            if (lesson == null)
            {
                return NotFound(new { Message = "Lesson not found" });
            }

            if (System.IO.File.Exists(lesson.FilePath))
            {
                try
                {
                    System.IO.File.Delete(lesson.FilePath);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = "Failed to delete the file", Details = ex.Message });
                }
            }

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
