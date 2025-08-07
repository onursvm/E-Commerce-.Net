namespace E_Commerce.Business.AuthServices.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateRefreshTokenAsync(int userId);
        Task<bool> ValidateRefreshTokenAsync(int userId, string refreshToken);
    }
}
