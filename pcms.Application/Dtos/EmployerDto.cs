using System.ComponentModel.DataAnnotations;

namespace pcms.Domain.Entities
{
    public class EmployerDto
    {
        [Key]
        public string EmployerId { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string TinNumber { get; set; }
        public ICollection<Member>? Members { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string PensionOfficer { get; set; }
        public string Status { get; set; } = "Inactive";
        public bool IsValid { get; set; } = false;
        public string Remarks { get; set; }

    }
}
