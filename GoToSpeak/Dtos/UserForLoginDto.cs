using System.ComponentModel.DataAnnotations;

namespace GoToSpeak.Dtos
{
    public class UserForLoginDto
    {
        [Required]
        public string Password {get;set;}
        [Required]
        public string Username { get; set; }
    }
}