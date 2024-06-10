using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.Models;
using NZWalks.Models.DTO;
using NZWalks.Repository;

namespace NZWalks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //Inject User manager and Token Repository
        private readonly UserManager<IdentityUser> _userManager;

        private readonly ITokenRepository _tokenRepository;
        public AuthController( UserManager<IdentityUser> userManager, ITokenRepository token)
        {
            
            _userManager = userManager;
            _tokenRepository = token;
        }
        //post: /api/Auth/Regitor

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.UserName,
            };

            var identityResult= await _userManager.CreateAsync(identityUser,registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                //Add role to user

                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                    if (identityResult.Succeeded)
                    {
                        return Ok("User was registered, Please login!!");
                    }
                }
            }

            return BadRequest("Something Error ocured!!");
        }

        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest) 
        {
            //check User Name 

            var user = await _userManager.FindByEmailAsync(loginRequest.UserName);

            if (user != null)
            {
                // check password

                var checkPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

                if (checkPassword)
                {
                    // Get Role for this user

                    var role = await _userManager.GetRolesAsync(user);
                    if (role != null)
                    {
                        //Create Token based on role 
                        var token=_tokenRepository.CreateJWTToken(user, role.ToList());

                        var response = new LoginResponseDto
                        {
                            JwtToken = token
                        };
                        return Ok(response);
                    }
                }
                
            }

            return BadRequest("UserName or Password incorrect!!");
        }

    }
}
