using SkinScan_Core.Contexts;

namespace SkinScan_API.Dtos
{
    public class RegisterUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int? Age { get; set; } 
        public  UserType userType { get; set; } 
    }
  

}
