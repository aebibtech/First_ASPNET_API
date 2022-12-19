using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MovieAPI.Data;
using MovieAPI.Models;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly CoreContext _context;

        public MoviesController(CoreContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> GetMovies([FromBody] MovieQuery query)
        {
            var result = from m in _context.Movie
                         select m;

            if (!string.IsNullOrEmpty(query.Title))
            {
                result = result.Where(m => m.Title!.Contains(query.Title));
            }

            if (!string.IsNullOrEmpty(query.Genre))
            {
                result = result.Where(m => m.Genre == query.Genre);
            }

            if (query.Id != 0)
            {
                result = result.Where(m => m.Id == query.Id);
            }

            var finalResult = await result.ToListAsync();

            return Ok(query.PageSize > 0 ? finalResult.Take(query.PageSize) : finalResult);
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateNewMovie([FromBody] Movie movie)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Movie.Add(movie);
            if (await _context.SaveChangesAsync() > 0)
                return Ok(movie);

            return BadRequest(ModelState);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateMovie([FromBody] MovieUpdate updates)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var movieToUpdate = await _context.Movie.Where(m => m.Id == updates.Id).FirstOrDefaultAsync();
            if (movieToUpdate == null)
                return NotFound();

            movieToUpdate.Title = updates.Title;
            movieToUpdate.ReleaseDate = updates.ReleaseDate;
            movieToUpdate.Genre = updates.Genre;
            movieToUpdate.Price = updates.Price;
            
            _context.Entry(movieToUpdate).State = EntityState.Modified;
            _context.Movie.Update(movieToUpdate);
            if (await _context.SaveChangesAsync() > 0)
                return Ok(movieToUpdate);
            else
                return BadRequest(ModelState);
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteMovie([FromRoute] int id)
        {
            var movieToDelete = await _context.Movie.Where(_ => _.Id == id).FirstOrDefaultAsync();
            if (movieToDelete == null)
                return NotFound();

            _context.Movie.Remove(movieToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
