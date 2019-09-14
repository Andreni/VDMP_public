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
    public class MoviesController : ControllerBase
    {
        private readonly VDMPContext _context;

        public MoviesController(VDMPContext context)
        {
            _context = context;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            try
            {
                return await _context.Movies.ToListAsync();
            }
            catch (SqlException)
            {
                return StatusCode(503, null);
            }
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(id);

                if (movie == null) return NotFound();

                return movie;
            }
            catch (SqlException)
            {
                return StatusCode(503, null);
            }
        }

        /// <summary>
        ///     Gets all the user ratings for the specific movie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Movies/AverageScore/5
        [HttpGet("AverageScore/{id}")]
        public async Task<ActionResult<List<int>>> GetRatingForMovie(int id)
        {
            try
            {
                var ratings = await _context.Movies.Where(x => x.TMDbId == id).Select(a => a.Rating).ToListAsync();

                if (ratings == null) return NotFound();
                return ratings;
            }
            catch (SqlException)
            {
                return StatusCode(503, null);
            }
        }

        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            try
            {
                if (id != movie.MovieId) return BadRequest();

                _context.Entry(movie).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(id))
                        return NotFound();
                    throw;
                }

                return NoContent();
            }
            catch (SqlException)
            {
                return StatusCode(503, null);
            }
        }

        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            try
            {
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetMovie", new {id = movie.MovieId}, movie);
            }
            catch (SqlException)
            {
                return StatusCode(503, null);
            }
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovie(int id)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(id);
                if (movie == null) return NotFound();

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                return movie;
            }
            catch (SqlException)
            {
                return StatusCode(503, NoContent());
            }
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
    }
}