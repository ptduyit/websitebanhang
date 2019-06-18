using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;

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
        public async Task<IActionResult> Provinces()
        {
            var provinces = await _context.Provinces.OrderBy(p => p.Name).ToListAsync();
            return Ok(new Response
            {
                Status = 200,
                Module = provinces
            });
        }

        // GET: api/LocationTree/5
        [HttpGet("{id}/district")]
        public async Task<IActionResult> GetDistricts(int id)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }
            var districts = await _context.Districts.Where(d => d.ProvinceId == id).OrderBy(d => d.Name).ToListAsync();
            return Ok(new Response
            {
                Status = 200,
                Module = districts
            });
        }
        [HttpGet("district/{id}/ward")]
        public async Task<IActionResult> GetWard(int id)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }
            var wards = await _context.Wards.Where(d => d.DistrictId == id).OrderBy(d => d.Name).ToListAsync();
            return Ok(new Response
            {
                Status = 200,
                Module = wards
            });
        }
    }
}
