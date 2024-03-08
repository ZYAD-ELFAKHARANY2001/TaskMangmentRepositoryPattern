namespace Task_Mangment_Api.DTO
{
    public class UserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; } // Store password hash, not the plain password
        public IFormFile? ProfilePicture { get; set; }
    }
}
