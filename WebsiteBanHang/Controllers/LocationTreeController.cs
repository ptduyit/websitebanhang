using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationTreeController : ControllerBase
    {
        private readonly SaleDBContext _context;
        public LocationTreeController(SaleDBContext context)
        {
            _context = context;
        }
        // GET: api/LocationTree
        [HttpGet]
        public IEnumerable<Provinces> Provinces()
        {
            return _context.Provinces.OrderBy(p => p.Name);
        }

        // GET: api/LocationTree/5
        [HttpGet("{id}/district")]
        public async Task<IActionResult> GetDistricts(int id)
        {
            var districts = await _context.Districts.Where(d => d.ProvinceId == id).OrderBy(d => d.Name).ToListAsync();
            if(districts == null)
            {
                return BadRequest();
            }
            return Ok(districts);
        }
        [HttpGet("district/{id}/ward")]
        public async Task<IActionResult> GetWard(int id)
        {
            var wards = await _context.Wards.Where(d => d.DistrictId == id).OrderBy(d => d.Name).ToListAsync();
            if (wards == null)
            {
                return BadRequest();
            }
            return Ok(wards);
        }
    }
}
