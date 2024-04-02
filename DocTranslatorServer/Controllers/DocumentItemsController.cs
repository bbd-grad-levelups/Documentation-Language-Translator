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
    public class DocumentItemsController : ControllerBase
    {
        private readonly DocumentsContext _context;

        public DocumentItemsController(DocumentsContext context)
        {
            _context = context;
        }

        // GET: api/DocumentItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Documents>>> GetDocumentsItems()
        {
            return await _context.DocumentsItems.ToListAsync();
        }

        // GET: api/DocumentItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Documents>> GetDocuments(long id)
        {
            var documents = await _context.DocumentsItems.FindAsync(id);

            if (documents == null)
            {
                return NotFound();
            }

            return documents;
        }

        // PUT: api/DocumentItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocuments(long id, Documents documents)
        {
            if (id != documents.Id)
            {
                return BadRequest();
            }

            _context.Entry(documents).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentsExists(id))
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

        // POST: api/DocumentItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Documents>> PostDocuments(Documents documents)
        {
            _context.DocumentsItems.Add(documents);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocuments", new { id = documents.Id }, documents);
        }

        // DELETE: api/DocumentItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocuments(long id)
        {
            var documents = await _context.DocumentsItems.FindAsync(id);
            if (documents == null)
            {
                return NotFound();
            }

            _context.DocumentsItems.Remove(documents);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DocumentsExists(long id)
        {
            return _context.DocumentsItems.Any(e => e.Id == id);
        }
    }
}
