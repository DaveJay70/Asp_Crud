using System.ComponentModel.DataAnnotations;
namespace Static_crud.Models
{
    public class UserModel
    {
       
        public int? UserID { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        [Required]
        [Phone]
        [MinLength(10)]
        [MaxLength(10)]
        public string MobileNo { get; set; }
        [Required]
        public string Address { get; set; }
       

        public bool IsActive  { get; set; } 
    }

    
    
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
   
}

