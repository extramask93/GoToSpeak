using System.ComponentModel.DataAnnotations;

namespace GoToSpeak.Dtos
{
    public class LoginWithRecoveryCodeDto
    {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; }
            public string UserName { get; set; }
    }
}