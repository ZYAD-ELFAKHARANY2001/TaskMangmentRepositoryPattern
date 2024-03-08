using System.ComponentModel.DataAnnotations;

namespace Task_Mangment_Api.DTO
{
    public class UserRegistirationModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Phone { get; internal set; }
        public IFormFile? ProfilePicture { get; set; }

    }
}
