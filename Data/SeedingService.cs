using SalesWebMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesWebMvc.Models.Enums;

namespace SalesWebMvc.Data
{
    public class SeedingService
    {
        private SalesWebMvcContext _context;

        public SeedingService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if (_context.Department.Any() || _context.Seller.Any() || _context.SalesRecord.Any())
            {
                return;
            }
            Department d1 = new Department( "Computers");
            Department d2 = new Department( "Devops");
            Department d3 = new Department( "Software Development");

            Seller s1 = new Seller( "Maro de Melo", "maro_fis@hotmail.com", 5000.00, new DateTime(1988, 01, 30), d3);

            SalesRecord sr1 = new SalesRecord( new DateTime(2023, 01, 23), 300.00, SaleStatus.Billed, s1);

            _context.Department.AddRange(d1, d2, d3);

            _context.Seller.AddRange(s1);

            _context.SalesRecord.AddRange(sr1);

            _context.SaveChanges();


        }
    }
}
