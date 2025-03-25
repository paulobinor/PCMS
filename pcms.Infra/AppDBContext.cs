using Microsoft.AspNetCore.Identity;
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
                new Member
                {
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Employer = "NDPR",
                    Phone = "23480102468635",
                    RegistrationDate = DateTime.Now,
                    IsDeleted = false,
                    RSAPin = "PIN1234567890"
                },
                new Member 
                { 
                    Name = "Jane Smith", 
                    Email = "jane.smith@example.com", 
                    DateOfBirth = new DateTime(1990, 8, 21),
                    Employer = "NNPC",
                    Phone = "2349876543210",
                    RegistrationDate = DateTime.Now,
                    IsDeleted = false,
                    RSAPin = "PIN9993243989"
                }
            );
            SeedRoles(modelBuilder);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { ConcurrencyStamp = "1", Name = "Admin", NormalizedName = "Admin" },
                new IdentityRole { ConcurrencyStamp = "2", Name = "User", NormalizedName = "User" },
                new IdentityRole { ConcurrencyStamp = "3", Name = "HR", NormalizedName = "HR" }
                );
        }
    }
}
