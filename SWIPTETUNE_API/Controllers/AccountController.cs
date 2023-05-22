using System.Text;
using BusinessObject;
using Repository.Repo;
using System.Xml.Linq;
using Repository.Interface;
using BusinessObject.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Sub_Model;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace SWIPTETUNE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountRepository repository = new AccountRepository();
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(RegisterAccountModel account)
        {
            string msg = "";
            try
            {
                var p = repository.RegisterAccount(account);
                msg = "Register Successfully";
                if (p == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                msg= ex.Message;
            }
             
                return Ok(
                     msg
                    );
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string email,string password) {
        
                var account = repository.Login(email, password);
            if(account==null)
            {
                return NotFound();
            }
            string msg = "Login successfully";
            var token = GenerateJwtToken(account.AccountId.ToString(),account.FullName);
            repository.AddToken(account.AccountId, token);

            return Ok(new
            {
                message=msg,
                token=token
            });
        }

        [HttpPut]
        [Route("update")]
        [Authorize(Policy = "Admin")]
        public IActionResult UpdateAccount(Guid Id,UpdateAccountModel account) {

            string msg = "";
            try
            {
                repository.UpdateAccount(Id, account);
                msg = "Update successfully";
            }
catch (Exception ex)
            {
                msg=ex.Message;
            }
            return Ok(new { message=msg });
        }
        private string GenerateJwtToken(string userId,string name)
        {
            // Define the security key (can be a string or a certificate)
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Key")));

            // Create signing credentials using the security key
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenIssuer = _configuration.GetValue<string>("Jwt:Issuer");
            var tokenAudience = _configuration.GetValue<string>("Jwt:Audience");

            // Set the claims for the token
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Name, name)
    };

            // Set the token expiration time
            var tokenExpiration = DateTime.UtcNow.AddHours(1);

            // Create the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpiration,
                SigningCredentials = signingCredentials,
                Issuer = tokenIssuer,
                Audience = tokenAudience
            };

            // Create the token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Generate the token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Serialize the token to a string
            var jwtToken = tokenHandler.WriteToken(token);

            return jwtToken;
        }
        [HttpDelete]
        [Route("delete")]
        [Authorize(Policy = "Admin")]
        public IActionResult DeleteAccount(Guid accountId)
        {
            string msg = "";
            try
            {
                repository.DeleteAccount(accountId);
                msg = "Delete Successfully";
            }catch(Exception ex)
            {
                msg= ex.Message;
            }
            return Ok(new
            {
                Message = msg,
            });
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Get the current user's account ID
                var accountId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Remove the access token from the account
                repository.LogOut(accountId);

                return Ok("Logged out successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
