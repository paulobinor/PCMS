using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcms.Domain.Entities
{
    public class Member
    {
        [Key]
        public string MemberId { get; set; } = Guid.NewGuid().ToString();
        public string Employer { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Contribution> Contributions { get; set; } = new List<Contribution>();
        public bool IsDeleted { get; set; } = false;
        public string RSAPin { get; set; } = $"PIN{new Random().Next(00000, 99999)}";
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public bool IsEligibleForBenefit { get; set; } = false;
    }
}
