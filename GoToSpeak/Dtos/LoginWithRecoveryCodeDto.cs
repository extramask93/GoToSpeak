using System.ComponentModel.DataAnnotations;

namespace GoToSpeak.Dtos
{
    public class LoginWithRecoveryCodeDto
    {
            [Required]
            [DataType(DataType.Text)]
            public string RecoveryCode { get; set; }
            [Required]
            public string UserName { get; set; }
    }
}