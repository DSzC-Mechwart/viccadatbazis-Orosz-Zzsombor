using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViccAdatbazis.Data;
using ViccAdatbazis.Models;
using System.Threading.Tasks;
using System.Linq;

namespace ViccAdatbazis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViccController : ControllerBase
    {
        private readonly ViccDbContext _context;

        public ViccController(ViccDbContext context)
        {
            _context = context;
        }

        // GET: api/Vicc?page=1
        [HttpGet]
        public async Task<IActionResult> GetViccek([FromQuery] int page = 1, int pageSize = 10)
        {
            var viccek = await _context.Viccek
                .Where(v => v.Aktiv) // Only active jokes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(viccek);
        }

        // GET: api/Vicc/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVicc(int id)
        {
            var vicc = await _context.Viccek.FindAsync(id);
            if (vicc == null || !vicc.Aktiv)
            {
                return NotFound();
            }

            return Ok(vicc);
        }

        // POST: api/Vicc
        [HttpPost]
        public async Task<IActionResult> PostVicc(Vicc vicc)
        {
            vicc.Aktiv = true; // New jokes are active by default
            _context.Viccek.Add(vicc);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVicc), new { id = vicc.Id }, vicc);
        }

        // PUT: api/Vicc/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVicc(int id, Vicc updatedVicc)
        {
            if (id != updatedVicc.Id)
            {
                return BadRequest();
            }

            var vicc = await _context.Viccek.FindAsync(id);
            if (vicc == null || !vicc.Aktiv)
            {
                return NotFound();
            }

            // Update joke content and uploader name
            vicc.Tartalom = updatedVicc.Tartalom;
            vicc.Feltolto = updatedVicc.Feltolto;

            _context.Entry(vicc).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Vicc/5 (archive the joke)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVicc(int id)
        {
            var vicc = await _context.Viccek.FindAsync(id);
            if (vicc == null || !vicc.Aktiv)
            {
                return NotFound();
            }

            // Archive the joke instead of deleting it permanently
            vicc.Aktiv = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Vicc/archived/5 (permanently delete archived joke)
        [HttpDelete("archived/{id}")]
        public async Task<IActionResult> DeleteArchivedVicc(int id)
        {
            var vicc = await _context.Viccek
                .Where(v => !v.Aktiv)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vicc == null)
            {
                return NotFound();
            }

            _context.Viccek.Remove(vicc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Vicc/like/5 (like a joke)
        [HttpPatch("like/{id}")]
        public async Task<IActionResult> LikeVicc(int id)
        {
            var vicc = await _context.Viccek.FindAsync(id);
            if (vicc == null || !vicc.Aktiv)
            {
                return NotFound();
            }

            vicc.Tetszik += 1; // Increment like count
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Vicc/dislike/5 (dislike a joke)
        [HttpPatch("dislike/{id}")]
        public async Task<IActionResult> DislikeVicc(int id)
        {
            var vicc = await _context.Viccek.FindAsync(id);
            if (vicc == null || !vicc.Aktiv)
            {
                return NotFound();
            }

            vicc.NemTetszik += 1; // Increment dislike count
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
