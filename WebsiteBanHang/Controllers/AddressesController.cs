using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;

namespace WebsiteBanHang.Controllers
{
    [Route("api/address")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly SaleDBContext _context;
        private readonly IMapper _mapper;

        public AddressesController(SaleDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById([FromRoute] int id)
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

            var address = await _context.Address.Where(a => a.AddressId == id).Include(a => a.Wards).ThenInclude(d => d.Districts).ThenInclude(p => p.Provinces).SingleOrDefaultAsync();

            if (address == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Message = "Địa chỉ không tồn tại",
                    Status = 404
                });
            }

            return Ok(new Response
            {
                Module = address,
                Status = 200
            });
        }
        [HttpGet("default/{id}")]
        public async Task<IActionResult> GetAddressDefault(Guid id)
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
            var address = await _context.Address.Where(a => a.UserId == id && a.IsDefault == true)
                .Include(a => a.Wards).ThenInclude(d => d.Districts).ThenInclude(p => p.Provinces).FirstOrDefaultAsync();
            if (address == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Message = "Người dùng không có địa chỉ nhận hàng",
                    Status = 404
                });
            }
            var address_map = _mapper.Map<AddressListViewModel>(address);
            return Ok(new Response
            {
                Status = 200,
                Module = address_map
            });
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAddressByUserId([FromRoute] Guid userId)
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
            var address = await _context.Address.Where(a => a.UserId == userId).Include(a=> a.Wards).ThenInclude(d => d.Districts).ThenInclude(p => p.Provinces).ToListAsync();
            if (!address.Any())
            {
                return Ok(new Response
                {
                    IsError = true,
                    Message = "Người dùng không có địa chỉ nhận hàng",
                    Status = 404
                });
            }
            var address_map = _mapper.Map<List<AddressListViewModel>>(address);
            return Ok(new Response
            {
                Status = 200,
                Module = address_map
            });
        }
        // PUT: api/Addresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress([FromRoute] int id, [FromBody] Address address)
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

            if (id != address.AddressId)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }

            var addressDefault = await _context.Address.Where(a => a.IsDefault == true && a.UserId == address.UserId).SingleOrDefaultAsync();
            //if(addressDefault == null)
            //{
                
            //}
            if (addressDefault.AddressId == id)
            {
                address.IsDefault = true;
                _context.Entry(addressDefault).CurrentValues.SetValues(address);
            }
            else if (address.IsDefault == true && addressDefault.AddressId != id)
            {
                addressDefault.IsDefault = false;
                _context.Entry(addressDefault).State = EntityState.Modified;
                _context.Entry(address).State = EntityState.Modified;
            }
            else
            {
                _context.Entry(address).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 404,
                        Message = "Không tìm thấy địa chỉ"
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

        // POST: api/Addresses
        [HttpPost]
        public async Task<IActionResult> PostAddress([FromBody] Address address)
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

            var addressDefault = await _context.Address.Where(a => a.IsDefault == true && a.UserId == address.UserId).SingleOrDefaultAsync();
            if(addressDefault != null && address.IsDefault == true)
            {
                addressDefault.IsDefault = false;
                _context.Entry(addressDefault).State = EntityState.Modified;
            }
            else if(address.IsDefault == false && addressDefault == null)
            {
                address.IsDefault = true;
            }
            _context.Address.Add(address);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 409,
                    Message = "Không thể thêm địa chỉ"
                });
            }

            return Ok(new Response
            {
                Status = 201,
                Module = address
            });
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress([FromRoute] int id)
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
            var address = await _context.Address.FindAsync(id);
            if (address == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy địa chỉ để xóa"
                });
            }
            if (address.IsDefault)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 409,
                    Message = "Không thể xóa địa chỉ mặc định"
                });
            }
      
            _context.Address.Remove(address);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 409,
                    Message = "Có lỗi khi xóa"
                });
            }

            return Ok(new Response
            {
                Status = 204                
            });
        }

        private bool AddressExists(int id)
        {
            return _context.Address.Any(e => e.AddressId == id);
        }
    }
}