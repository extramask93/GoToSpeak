using System;

namespace GoToSpeak.Dtos
{
    public class PasswordForResetDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}