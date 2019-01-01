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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly SaleDBContext _context;

        public AddressesController(SaleDBContext context)
        {
            _context = context;
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var address = await _context.Address.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAddressByUserId([FromRoute] Guid userId)
        {
            var address = await _context.Address.Where(a => a.UserId == userId).ToListAsync();
            if (address == null)
            {
                return NotFound();
            }
            return Ok(address);
        }
        // PUT: api/Addresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress([FromRoute] int id, [FromBody] Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != address.AddressId)
            {
                return BadRequest();
            }

            var addressDefault = await _context.Address.Where(a => a.IsDefault == true).SingleOrDefaultAsync();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Addresses
        [HttpPost]
        public async Task<IActionResult> PostAddress([FromBody] Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var addressDefault = await _context.Address.Where(a => a.IsDefault == true).SingleOrDefaultAsync();
            if(address.IsDefault == true && addressDefault != null)
            {
                addressDefault.IsDefault = false;
                _context.Entry(addressDefault).State = EntityState.Modified;
            }
            else if(address.IsDefault == false && addressDefault == null)
            {
                address.IsDefault = true;
            }
            _context.Address.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAddressById", new { id = address.AddressId }, address);
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var addressDefault = await _context.Address.Where(a => a.IsDefault == true).SingleOrDefaultAsync();
            if(addressDefault.AddressId == id)
            {
                return BadRequest(ModelState);
            }
            var address = await _context.Address.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _context.Address.Remove(address);
            await _context.SaveChangesAsync();

            return Ok(address);
        }

        private bool AddressExists(int id)
        {
            return _context.Address.Any(e => e.AddressId == id);
        }
    }
}