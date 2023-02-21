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

        public List<Seller> FindAll()
        {
            return  _context.Seller.ToList();
        }

        public void Insert(Seller seller)
        {           
            _context.Add(seller);
            _context.SaveChanges();
        }

        public Seller FindById(int id)
        {
            var seller = _context.Seller.Include(s => s.Department).Where(s => s.Id == id).FirstOrDefault();

            return seller;
        }

        public void Remove(int id)
        {
            var seller = _context.Seller.Find(id);
            _context.Seller.Remove(seller);
            _context.SaveChanges();
        }

        public void Update(Seller seller)
        {
            if(!_context.Seller.Any(s=>s.Id == seller.Id))
            {
                throw new NotFoundException("Id not found");
            }
            try { 
            _context.Update(seller);
            _context.SaveChanges();
            }
            catch(DbUpdateConcurrencyException e)
            {
               throw new DbConcurrencyException(e.Message);
            }
        }
    }
}
