using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocTranslatorServer.Models;

namespace DocTranslatorServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly LanguageContext _context;

        public LanguageController(LanguageContext context)
        {
            _context = context;
        }

        // GET: api/Language
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Language>>> GetLanguage()
        {
            return await _context.Language.ToListAsync();
        }

        // GET: api/Language/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Language>> GetLanguage(int id)
        {
            var language = await _context.Language.FindAsync(id);

            if (language == null)
            {
                return NotFound();
            }

            return language;
        }

        // PUT: api/Language/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLanguage(int id, Language language)
        {
            if (id != language.LanguageID)
            {
                return BadRequest();
            }

            _context.Entry(language).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LanguageExists(id))
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

        // POST: api/Language
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Language>> PostLanguage(Language language)
        {
            _context.Language.Add(language);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLanguage", new { id = language.LanguageID }, language);
        }

        // DELETE: api/Language/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            var language = await _context.Language.FindAsync(id);
            if (language == null)
            {
                return NotFound();
            }

            _context.Language.Remove(language);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LanguageExists(int id)
        {
            return _context.Language.Any(e => e.LanguageID == id);
        }
    }
}
