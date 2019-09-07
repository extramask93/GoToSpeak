using System.ComponentModel.DataAnnotations;

namespace GoToSpeak.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(255,MinimumLength=8, ErrorMessage="Please specify password length between 4 and 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W]).{8,}$", ErrorMessage = "Your password should have at least one upper case letter, lower case letter, number and a special character .")]
        public string Password {get;set;}
        [Required]
        [StringLength(255,MinimumLength=8, ErrorMessage="Please specify password length between 4 and 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W]).{8,}$", ErrorMessage = "Your password should have at least one upper case letter, lower case letter, number and a special character .")]
        public string ConfirmPassword { get; set; }
        [Required]
        [StringLength(255,MinimumLength=4, ErrorMessage="Username should be at least 4 character long")]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email {get; set;}
        [Phone]
        public string Phone { get; set; }
    }
}