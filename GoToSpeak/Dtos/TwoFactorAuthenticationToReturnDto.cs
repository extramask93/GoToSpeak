namespace GoToSpeak.Dtos
{
    public class TwoFactorAuthenticationToReturnDto
    {
        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool Is2faEnabled { get; set; }
    }
}