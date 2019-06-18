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
    [Route("api/admin/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly SaleDBContext _context;

        public SuppliersController(SaleDBContext context)
        {
            _context = context;
        }

        // GET: api/Suppliers
        [HttpGet]
        public IEnumerable<Suppliers> GetSuppliers()
        {
            return _context.Suppliers;
        }

        // GET: api/Suppliers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSuppliers([FromRoute] int id)
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

            var suppliers = await _context.Suppliers.FindAsync(id);

            if (suppliers == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }
            return Ok(new Response
            {
                Status = 200,
                Module = suppliers
            });

        }
        [HttpGet("search/{keyword}")]
        public async Task<IActionResult> SearchSupplier([FromRoute] string keyword)
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
            var searchString = keyword.Split(' ');
            searchString = searchString.Select(x => x.ToLower()).ToArray();
            var rs = await _context.Suppliers.Where(p => searchString.All(s => p.CompanyName.ToLower().Contains(s))).Select(x => new
            {
                x.SupplierId,
                x.CompanyName
            }).ToListAsync();
            return Ok(rs);
        }
        // PUT: api/Suppliers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSuppliers([FromRoute] int id, [FromBody] Suppliers suppliers)
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

            if (id != suppliers.SupplierId)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }

            _context.Entry(suppliers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SuppliersExists(id))
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 404,
                        Message = "Không tìm thấy dữ liệu"
                    });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new Response
            {
                Status = 204
            });
        }

        // POST: api/Suppliers
        [HttpPost]
        public async Task<IActionResult> PostSuppliers([FromBody] Suppliers suppliers)
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

            _context.Suppliers.Add(suppliers);
            await _context.SaveChangesAsync();
            return Ok(new Response
            {
                Status = 201,
                Module = new { id = suppliers.SupplierId, name = suppliers.CompanyName }
            });
        }

        // DELETE: api/Suppliers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuppliers([FromRoute] int id)
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

            var suppliers = await _context.Suppliers.FindAsync(id);
            if (suppliers == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }

            _context.Suppliers.Remove(suppliers);
            await _context.SaveChangesAsync();

            return Ok(new Response
            {
                Status = 204
            });
        }

        private bool SuppliersExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}