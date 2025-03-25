using System.ComponentModel.DataAnnotations;

namespace pcms.Domain.Entities
{
    public class UserLogin
    {
        [Required(ErrorMessage = "UserName is required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

    }
}
