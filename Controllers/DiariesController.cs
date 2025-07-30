using Microsoft.AspNetCore.Mvc;
using lulu_diary_backend.Models.Database;
using Microsoft.EntityFrameworkCore;
using lulu_diary_backend.Context;
using lulu_diary_backend.Models.API;
namespace lulu_diary_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiariesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiariesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/diaries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Diary>>> GetDiaries()
        {
            return await _context.Diaries.OrderBy(p => p.Id).ToListAsync();
        }

        // GET: api/diaries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Diary>> GetDiary(int id)
        {
            var diary = await _context.Diaries.FindAsync(id);

            if (diary == null)
            {
                return NotFound();
            }

            return diary;
        }

        // POST: api/diaries
        [HttpPost]
        public async Task<ActionResult<Diary>> PostDiary(DiaryDto diary)
        {
            var newDiary = new Diary
            {
                Title = diary.Title,
                Content = diary.Content,
                Username = diary.Username,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Diaries.Add(newDiary);
            await _context.SaveChangesAsync();

            return Ok(newDiary);
        }

        // PATCH: api/diaries/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchDiary(int id, DiaryUpdateDto diaryUpdate)
        {
            var existingDiary = await _context.Diaries.FindAsync(id);
            if (existingDiary == null)
            {
                return NotFound();
            }

            // Update only the provided fields
            if (!string.IsNullOrEmpty(diaryUpdate.Title))
            {
                existingDiary.Title = diaryUpdate.Title;
            }
            
            if (!string.IsNullOrEmpty(diaryUpdate.Content))
            {
                existingDiary.Content = diaryUpdate.Content;
            }

            existingDiary.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Diaries.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(existingDiary);
        }

        // DELETE: api/diaries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiary(int id)
        {
            var diary = await _context.Diaries.FindAsync(id);
            if (diary == null)
            {
                return NotFound();
            }

            _context.Diaries.Remove(diary);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
