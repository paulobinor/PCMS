using Microsoft.EntityFrameworkCore;
using pcms.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcms.Infra
{
    public class AppDBContext : DbContext
    {
        
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        public DbSet<Contribution> Contributions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           // modelBuilder.Entity<Contribution>()
               // .HasOne(c => c.Member)
               // .WithMany(m => m.Contributions)
               // .HasForeignKey(c => c.MemberId)
              //  .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Member>().HasData(
                new Member {Name = "John Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(1985, 5, 15) },
                new Member {Name = "Jane Smith", Email = "jane.smith@example.com", DateOfBirth = new DateTime(1990, 8, 21) }
            );
        }
    }
}
