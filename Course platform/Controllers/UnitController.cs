using Course_platform.Models.DTO;
using Course_platform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Course_platform.Controllers
{
    [ApiController]
    [Route("api")]
    public class UnitController : ControllerBase
    {
        private readonly CoursePlatformDbContext _context;

        public UnitController(CoursePlatformDbContext context)
        {
            _context = context;
        }

        // Создание юнита
        [Authorize]
        [HttpPost("course/{courseId}/unit")]
        public async Task<IActionResult> CreateUnit(int courseId, [FromBody] CreateUnitDTO createUnitDto)
        {
            // Проверяем, существует ли курс
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound(new { Message = "Курс не найден." });
            }

            // Проверяем, передан ли заголовок юнита
            if (string.IsNullOrWhiteSpace(createUnitDto.Title))
            {
                return BadRequest(new { Message = "Заголовок юнита не может быть пустым." });
            }

            // Получаем текущий максимальный индекс OrderIndex для данного курса
            var maxOrderIndex = await _context.Units
                .Where(u => u.CourseId == courseId)
                .MaxAsync(u => (int?)u.OrderIndex) ?? 0;

            // Создаем юнит с автоматически увеличенным OrderIndex
            var unit = new UnitEntity
            {
                CourseId = courseId,
                OrderIndex = maxOrderIndex + 1,
                Title = createUnitDto.Title
            };

            _context.Units.Add(unit);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUnitById), new { unitId = unit.UnitId }, new
            {
                unit.UnitId,
                unit.CourseId,
                unit.OrderIndex,
                unit.Title
            });
        }

        // Получение юнита по ID
        [HttpGet("course/unit/{unitId}")]
        public async Task<IActionResult> GetUnitById(int unitId)
        {
            // Ищем юнит в базе данных
            var unit = await _context.Units
                .FirstOrDefaultAsync(u => u.UnitId == unitId);

            // Если юнит не найден
            if (unit == null)
            {
                return NotFound(new { Message = "Юнит не найден." });
            }

            // Возвращаем найденный юнит
            return Ok(new
            {
                unit.UnitId,
                unit.CourseId,
                unit.OrderIndex,
                unit.Title
            });
        }

        // Получение списка юнитов по ID курса
        [HttpGet("course/{courseId}/units")]
        public async Task<IActionResult> GetUnitsByCourseId(int courseId)
        {
            // Проверяем, существует ли курс
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound(new { Message = "Курс не найден." });
            }

            // Получаем список юнитов для данного курса, отсортированных по OrderIndex
            var units = await _context.Units
                .Where(u => u.CourseId == courseId)
                .OrderBy(u => u.OrderIndex)
                .Select(u => new
                {
                    u.UnitId,
                    u.CourseId,
                    u.OrderIndex,
                    u.Title
                })
                .ToListAsync();

            return Ok(units);
        }

        [Authorize]
        [HttpPut("course/{courseId}/unit/{unitId}")]
        public async Task<IActionResult> UpdateUnit(int courseId, int unitId, [FromBody] UpdateUnitDTO updateUnitDto)
        {
            // Проверяем, существует ли курс
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound(new { Message = "Курс не найден." });
            }

            // Проверяем, существует ли юнит
            var unit = await _context.Units
                .FirstOrDefaultAsync(u => u.UnitId == unitId && u.CourseId == courseId);

            if (unit == null)
            {
                return NotFound(new { Message = "Юнит не найден." });
            }

            // Обновляем название и порядок, если они переданы
            if (!string.IsNullOrWhiteSpace(updateUnitDto.Title))
            {
                unit.Title = updateUnitDto.Title;
            }

            if (updateUnitDto.OrderIndex.HasValue)
            {
                // Меняем порядок юнитов
                var existingUnitWithSameOrder = await _context.Units
                    .FirstOrDefaultAsync(u => u.CourseId == courseId && u.OrderIndex == updateUnitDto.OrderIndex);

                if (existingUnitWithSameOrder != null && existingUnitWithSameOrder.UnitId != unitId)
                {
                    existingUnitWithSameOrder.OrderIndex = unit.OrderIndex; // Обмениваемся индексами
                }

                unit.OrderIndex = updateUnitDto.OrderIndex.Value;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                unit.UnitId,
                unit.CourseId,
                unit.OrderIndex,
                unit.Title
            });
        }


        [Authorize]
        [HttpDelete("course/{courseId}/unit/{unitId}")]
        public async Task<IActionResult> DeleteUnit(int courseId, int unitId)
        {
            // Ищем курс в базе данных
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound(new { Message = "Курс не найден." });
            }

            // Ищем юнит по его ID
            var unit = await _context.Units
                .FirstOrDefaultAsync(u => u.UnitId == unitId && u.CourseId == courseId);

            // Если юнит не найден
            if (unit == null)
            {
                return NotFound(new { Message = "Юнит не найден." });
            }

            // Удаляем юнит
            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();

            // Возвращаем успешный ответ (204 No Content)
            return NoContent();
        }

        // Обновление порядка юнитов для курса
        [Authorize]
        [HttpPost("course/{courseId}/units/reorder")]
        public async Task<IActionResult> ReorderUnits(int courseId, [FromBody] List<ReorderUnitDTO> reorderUnitDtos)
        {
            // Проверяем, существует ли курс
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound(new { Message = "Курс не найден." });
            }

            // Проверяем, что передан правильный список юнитов с новыми порядками
            if (reorderUnitDtos == null || reorderUnitDtos.Count == 0)
            {
                return BadRequest(new { Message = "Список юнитов для обновления порядка не может быть пустым." });
            }

            // Получаем все юниты курса
            var units = await _context.Units
                .Where(u => u.CourseId == courseId)
                .ToListAsync();

            // Обновляем порядок юнитов в базе данных
            foreach (var reorderUnitDto in reorderUnitDtos)
            {
                var unit = units.FirstOrDefault(u => u.UnitId == reorderUnitDto.UnitId);
                if (unit != null)
                {
                    unit.OrderIndex = reorderUnitDto.NewOrderIndex;
                }
            }

            // Обновляем данные в базе
            await _context.SaveChangesAsync();

            // Возвращаем обновленные юниты с новым порядком
            var updatedUnits = units
                .OrderBy(u => u.OrderIndex)
                .Select(u => new
                {
                    u.UnitId,
                    u.CourseId,
                    u.OrderIndex,
                    u.Title
                })
                .ToList();

            return Ok(updatedUnits);
        }

    }
}
