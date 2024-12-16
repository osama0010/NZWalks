using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var IdentityUser = new IdentityUser
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.UserName
            };

            var IdentityResult = await userManager.CreateAsync(IdentityUser, registerRequestDto.Password);


            if (IdentityResult.Succeeded)
            {
                // Assigning Roles to User

                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    IdentityResult = await userManager.AddToRolesAsync(IdentityUser, registerRequestDto.Roles);

                    if(IdentityResult.Succeeded)
                    {
                        return Ok("User is register!! SignIn Now");
                    }
                }
            }
            return BadRequest("Something Went Wrong");
        }


        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        { 
            var user = await userManager.FindByEmailAsync(loginRequest.Username);
            if (user != null)
            {
                var checkPassword = await userManager.CheckPasswordAsync(user ,loginRequest.Password);
                if (checkPassword)
                {
                    // Create a Token
                    var roles = await userManager.GetRolesAsync(user);

                    if (roles != null)
                    {
                        var jwtToken = tokenRepository.CreateJwtToken(user, roles.ToList());

                        var response = new LoginResponseDto()
                        {
                            JwtToken = jwtToken
                        };

                        return Ok(response);
                    }
                    //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJyZWFkZXJAbnp3LmNvbSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlJlYWRlciIsImV4cCI6MTczNDM2NzQ0NSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA3My8iLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MDczLyJ9.xb1R4dmtXsiwLQ9xcjHqW-hXKT4OLHPdGE_VsRcxZ34
                }
            }

            return BadRequest("Username or Password are incorrect");

        }


    }
}
