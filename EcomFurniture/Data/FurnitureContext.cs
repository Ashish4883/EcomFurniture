using EcomFurniture.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcomFurniture.Data
{
    public class FurnitureContext:DbContext
    {
        public FurnitureContext(DbContextOptions<FurnitureContext> options):base(options)
        {

        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }

    }
}
