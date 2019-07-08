using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Helpers;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin,employee")]
    public class SlideShowsController : ControllerBase
    {
        private readonly SaleDBContext _context;
        private readonly IHostingEnvironment _environment;

        public SlideShowsController(SaleDBContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/SlideShows
        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<SlideShow> GetSlideShow()
        {
            return _context.SlideShow;
        }

        // GET: api/SlideShows/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSlideShow([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var slideShow = await _context.SlideShow.FindAsync(id);

            if (slideShow == null)
            {
                return NotFound();
            }

            return Ok(slideShow);
        }

        // PUT: api/SlideShows/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSlideShow([FromRoute] int id, [FromBody] SlideShow slideShow)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != slideShow.SlideId)
            {
                return BadRequest();
            }

            _context.Entry(slideShow).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SlideShowExists(id))
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

        // POST: api/SlideShows
        [HttpPost]
        public async Task<IActionResult> PostSlideShow([FromForm] List<IFormFile> file, [FromForm] string link)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var imageList = await Files.UploadAsync(file, _environment.ContentRootPath);
            var slide = new SlideShow
            {
                Link = link,
                Image = imageList[0]
            };
            _context.SlideShow.Add(slide);
            await _context.SaveChangesAsync();
            return StatusCode(201);

        }

        // DELETE: api/SlideShows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlideShow([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var slideShow = await _context.SlideShow.FindAsync(id);
            if (slideShow == null)
            {
                return NotFound();
            }
            Files.Delete(slideShow.Image, _environment.ContentRootPath);
            _context.SlideShow.Remove(slideShow);
            await _context.SaveChangesAsync();

            return Ok(slideShow);
        }

        private bool SlideShowExists(int id)
        {
            return _context.SlideShow.Any(e => e.SlideId == id);
        }
    }
}