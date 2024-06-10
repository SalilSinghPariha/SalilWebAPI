using Microsoft.AspNetCore.Identity;

namespace NZWalks.Repository
{
    public interface ITokenRepository
    {
       string CreateJWTToken(IdentityUser user, List<string> Roles);
    }
}
