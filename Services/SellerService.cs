using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models;
using SalesWebMvc.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.Services
{
    public class SellerService
    {
        private readonly SalesWebMvcContext _context;

        public SellerService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<List<Seller>> FindAll()
        {
            var sellers = await _context.Seller.ToListAsync();
            return sellers;
        }

        public async Task Insert(Seller seller)
        {
            await _context.AddAsync(seller);
            await _context.SaveChangesAsync();
        }

        public async Task<Seller> FindById(int id)
        {
            var seller = await _context.Seller.Include(s => s.Department).Where(s => s.Id == id).FirstOrDefaultAsync();

            return seller;
        }

        public async Task Remove(int id)
        {
            try { 
            var seller = await _context.Seller.FindAsync(id);
             _context.Seller.Remove(seller);
            await _context.SaveChangesAsync();
            }
            catch (IntegrityException e )
            {
                throw new IntegrityException(e.Message);
            }
        }

        public async Task Update(Seller seller)
        {
            var hasAny = await _context.Seller.AnyAsync(s => s.Id == seller.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id not found");
            }
            try
            {
               _context.Update(seller);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}
