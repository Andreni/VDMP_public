using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VDMP.DataAccess;
using VDMP.DBmodel;

namespace VDMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrariesController : ControllerBase
    {
        private readonly VDMPContext _context;

        public LibrariesController(VDMPContext context)
        {
            _context = context;
        }


        // GET: api/Libraries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Library>>> GetLibraries()
        {
            {
                return await _context.Libraries.Include(m => m.LibraryOfMovies).ToListAsync().ConfigureAwait(true);
            }
        }


        // GET: api/Libraries/UsersLibraries/35d0fc06-e822-4922-8420-7ba6c3767964
        [HttpGet("UsersLibraries/{userId}")]
        public async Task<ActionResult<IEnumerable<Library>>> GetLibrariesForUser(string userId)
        {
            try
            {
                return await _context.Libraries.Include(m => m.LibraryOfMovies).Where(x => x.UserId == userId)
                    .ToListAsync().ConfigureAwait(true);
            }
            catch (SqlException)
            {
                return StatusCode(503, null);
            }
        }


        // GET: api/Libraries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Library>> GetLibrary(int id)
        {
            try
            {
                var library = await _context.Libraries.FindAsync(id);
                if (library == null) return NotFound();

                await _context.Entry(library).Collection(x => x.LibraryOfMovies).LoadAsync();
                return library;
            }
            catch (SqlException)
            {
                return StatusCode(503, null);
            }
        }

        // PUT: api/Libraries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLibrary(int id, Library library)
        {
            try
            {
                if (id != library.LibraryId) return BadRequest();

                _context.Entry(library).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryExists(id)) return NotFound();
                }
            }
            catch (SqlException)
            {
                return StatusCode(503, null);
            }

            return NoContent();
        }

        // POST: api/Libraries
        [HttpPost]
        public async Task<ActionResult<Library>> PostLibrary(Library library)
        {
            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLibrary", new {id = library.LibraryId}, library);
        }

        // DELETE: api/Libraries/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Library>> DeleteLibrary(int id)
        {
            try
            {
                var library = await _context.Libraries.FindAsync(id);
                if (library == null) return NotFound();

                _context.Libraries.Remove(library);
                await _context.SaveChangesAsync();

                return library;
            }
            catch (SqlException)
            {
                return StatusCode(503, null);
            }
        }

        private bool LibraryExists(int id)
        {
            return _context.Libraries.Any(e => e.LibraryId == id);
        }
    }
}