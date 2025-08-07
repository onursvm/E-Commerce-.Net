namespace E_Commerce.Business.AuthServices.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(string email, string password);
        Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken);
        Task<bool> RevokeTokenAsync(string token);
        Task InitiatePasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}
