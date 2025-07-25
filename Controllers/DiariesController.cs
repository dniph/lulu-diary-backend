using Microsoft.AspNetCore.Mvc;
using lulu_diary_backend.Models.Database;
using Microsoft.EntityFrameworkCore;
using lulu_diary_backend.Context;
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
            return await _context.Diaries.ToListAsync();
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
        public async Task<ActionResult<Diary>> PostDiary(Diary diary)
        {
            _context.Diaries.Add(diary);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiary", new { id = diary.Id }, diary);
        }

        // PUT: api/diaries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiary(int id, Diary diary)
        {
            if (id != diary.Id)
            {
                return BadRequest();
            }

            _context.Entry(diary).State = EntityState.Modified;

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

            return NoContent();
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
