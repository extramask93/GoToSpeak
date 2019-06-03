using System.ComponentModel.DataAnnotations;

namespace GoToSpeak.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(8,MinimumLength=4, ErrorMessage="Please specify password length between 4 and 8 characters")]
        public string Password {get;set;}
        [Required]
        public string Username { get; set; }
    }
}