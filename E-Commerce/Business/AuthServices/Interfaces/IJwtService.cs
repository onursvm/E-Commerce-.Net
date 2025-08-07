using E_Commerce.DataAccses.Entities.Identity;
using System.Security.Claims;

namespace E_Commerce.Business.AuthServices.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user, IEnumerable<Role> roles);
        ClaimsPrincipal? GetPrincipalFromToken(string token);
    }
}
