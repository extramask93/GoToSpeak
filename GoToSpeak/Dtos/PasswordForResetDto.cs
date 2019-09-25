using System;
using System.ComponentModel.DataAnnotations;

namespace GoToSpeak.Dtos
{
    public class PasswordForResetDto
    {
        public string UserName { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(255,MinimumLength=8, ErrorMessage="Please specify password length between 4 and 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W]).{8,}$", ErrorMessage = "Your password should have at least one upper case letter, lower case letter, number and a special character .")]
        public string Password { get; set; }
        [Required]
        [StringLength(255,MinimumLength=8, ErrorMessage="Please specify password length between 4 and 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W]).{8,}$", ErrorMessage = "Your password should have at least one upper case letter, lower case letter, number and a special character .")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}