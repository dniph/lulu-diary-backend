using Microsoft.AspNetCore.Mvc;
using lulu_diary_backend.Models.Database;
using lulu_diary_backend.Context;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Repositories;

namespace lulu_diary_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiariesController : ControllerBase
    {
        private readonly DiariesRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiariesController"/> class.
        /// </summary>
        /// <param name="repository">Diaries repository.</param>
        public DiariesController(DiariesRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Gets a list of all diaries.
        /// </summary>
        /// <returns>List of diaries.</returns>
        [HttpGet]
        public async Task<IActionResult> ListAsync()
        {
            var results = await _repository.GetDiariesAsync();
            return Ok(results);
        }

        /// <summary>
        /// Creates a new diary.
        /// </summary>
        /// <param name="diary">Diary data transfer object.</param>
        /// <returns>Created diary.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(DiaryDto diary)
        {
            if (diary == null)
            {
                return BadRequest();
            }

            var result = await _repository.InsertDiaryAsync(diary);
            return Ok(result);
        }

        /// <summary>
        /// Gets a diary by its ID.
        /// </summary>
        /// <param name="id">Diary ID.</param>
        /// <returns>Diary if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var result = await _repository.GetDiaryAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing diary.
        /// </summary>
        /// <param name="id">Diary ID.</param>
        /// <param name="diary">Diary update data transfer object.</param>
        /// <returns>Updated diary if found, otherwise NotFound.</returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, DiaryUpdateDto diary)
        {
            if (diary == null)
            {
                return BadRequest();
            }

            var existingDiary = await _repository.UpdateDiaryAsync(id, diary);
            if (existingDiary == null)
            {
                return NotFound();
            }

            return Ok(existingDiary);
        }

        /// <summary>
        /// Deletes a diary by its ID.
        /// </summary>
        /// <param name="id">Diary ID.</param>
        /// <returns>NoContent if deleted, otherwise NotFound.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var diary = await _repository.DeleteDiaryAsync(id);

            if (diary == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
