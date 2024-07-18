using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using MyWebAPP.Models;
using Serilog;
using MyWebAPP.DTOs;

namespace MyWebAPP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly JwtOptions _jwtOptions;
        private readonly LibraryContext _dbContext;

        public UserController(JwtOptions jwtOptions, LibraryContext dbContext)
        {
            _jwtOptions = jwtOptions;
            _dbContext = dbContext;
        }

        [HttpPost("auth")]
        public ActionResult<string> AuthenticateUser(AuthenticationRequest request)
        {
            var user = _dbContext.Set<User>().FirstOrDefault(x => x.Username == request.Username && x.Password == request.Password);
            if (user == null)
            {
                Log.Warning("Authentication failed for user {Username}", request.Username);
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.issuer,
                Audience = _jwtOptions.audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.signingKey)), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            Log.Information("User {Username} authenticated successfully", user.Username);
            return Ok(accessToken);
        }
    }
}
