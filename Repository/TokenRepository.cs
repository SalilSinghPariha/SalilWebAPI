using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NZWalks.Repository
{
    public class TokenRepository : ITokenRepository
    {
        //In order to access the configuration object or value here we need to inject same so
        //we can do that using constructor

      private readonly IConfiguration _configuration;

        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateJWTToken(IdentityUser user, List<string> Roles)
        {
            // use JWT token usinf JWT repository

            // create Claim

            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            /// now add the roles in claim collection
            
            foreach (var role in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // get the key to for credential

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            // using HmacSha256 security algorithms to sign credential

            var credential = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            // Now create JWT token using issuer and audience with expires and above key and crdential

            var token = new JwtSecurityToken
            (
                 _configuration["Jwt:Issuer"], //we can get this from appsetting.json
                _configuration["Jwt:Audience"], //we can get this from appsetting.json since we have used Iconfiguration and injected the same under this class
                claims, // passed the claims
                expires: DateTime.Now.AddMinutes(15), // for token expiry
                signingCredentials: credential //with SHA algo
                );

            // this will return new token in string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
