namespace pcms.Application.Dto
{
    public class AddMemberDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? RSAPin { get; set; }
        public string Employer { get; set; }
    }
}
