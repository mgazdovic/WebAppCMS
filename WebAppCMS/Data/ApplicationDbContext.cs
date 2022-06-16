using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebAppCMS.Models;

namespace WebAppCMS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        /* TODO: dodati seed u migraciju! (Entity.HasData ...) za jednog admin i jednog obicnog usera */

        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }
    }
}
