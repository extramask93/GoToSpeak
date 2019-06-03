using System.ComponentModel.DataAnnotations;

namespace GoToSpeak.Dtos
{
    public class UserForLoginDto
    {

        public string Password {get;set;}
        public string Username { get; set; }
    }
}