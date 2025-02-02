using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace LMS.Models
{
    public class AppUser : IdentityUser
    {
        public decimal Balance { get; set; }
        public string? Img { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Genders Gender { get; set; }
        public enum Genders { Male, Female }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Account number must be a positive integer.")]
        public int AccountNumber { get; set; } = GenerateAccountNumber();
        public static int GenerateAccountNumber()
        {
            var random = new Random();
            return random.Next(1, int.MaxValue);
        }
        [Required(ErrorMessage = "Enter Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Enter Civil ID")]
        [MinLength(12)]
        [MaxLength(12)]
        public string CivilID { get; set; }

        //[Required]
        //[DisplayName("Role ID")]
        //[ForeignKey("Role")]
        //public string RoleId { get; set; } // Foreign Key to Role
        //public IdentityRole? Role { get; set; } // Navigation property
    }
}
