namespace E_Commerce.Presentation.Dtos.Auth
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TokenDto Token { get; set; }
        public UserInfoDto User { get; set; }
    }
}
