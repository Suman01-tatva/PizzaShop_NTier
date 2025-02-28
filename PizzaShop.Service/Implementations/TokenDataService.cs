using PizzaShop.Service.Interfaces;
using System.Security.Claims;

namespace Pizzashop.Service.Implementations;

public class TokenDataService : ITokenDataService
{
    private readonly IJwtService _jwtservice;

    public TokenDataService(IJwtService jwtservice)
    {
        _jwtservice = jwtservice;
    }

    public async Task<string> GetEmailFromToken(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            var principal = _jwtservice.ValidateToken(token);
            if (principal != null)
            {
                var emailClaim = principal.Claims.First(claim => claim.Type == ClaimTypes.Email);
                var email = emailClaim.Value;

                return email.ToString();
            }
        }

        return "";
    }
}
