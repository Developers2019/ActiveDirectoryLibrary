using System.ComponentModel.DataAnnotations;

namespace ActiveDirectoryLibrary.Model
{
    public class RegisterViewModel
    {
        public string ADUsername { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;
        
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        public string Name => $"{FullName} {LastName}";

        public bool Error { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;


    }
}