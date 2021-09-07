using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Implement your model overiding logic here.
            //Example
            //builder.Entity<CardAttachment>()
            //    .Property(p => p.SizeInKB)
            //    .HasColumnType("decimal(19,2)");
        }

        //Add your DbSets here
    }
}
