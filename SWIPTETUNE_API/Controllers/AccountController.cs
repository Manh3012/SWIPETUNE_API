using MimeKit;
using System.Text;
using BusinessObject;
using Repository.Repo;
using MailKit.Security;
using Repository.Interface;
using DataAccess.Interface;
using BusinessObject.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Sub_Model;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using static SpotifyAPI.Web.PlaylistRemoveItemsRequest;

namespace SWIPTETUNE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountRepository repository;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<Account> _signInManager;
        private readonly UserManager<Account> _userManager;
        private readonly MailSettings mailSettings;
        private readonly ISpotifyService spotifyService;
        private readonly SWIPETUNEDbContext context;
        private readonly ISubscriptionRepository subscriptionRepository;

        public AccountController(IConfiguration configuration, IAccountRepository accountRepository, SignInManager<Account> signInManager, UserManager<Account> userManager, IOptions<MailSettings> _mailSettings, ISpotifyService spotifyService, SWIPETUNEDbContext context, ISubscriptionRepository subscriptionRepository)
        {
            _configuration = configuration;
            repository = accountRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            mailSettings = _mailSettings.Value;
            this.spotifyService = spotifyService;
            this.context = context;
            this.subscriptionRepository = subscriptionRepository;
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterAccountModel model)
        {
            var user = new Account
            {
                UserName = model.Email,
                Email = model.Email,
                DOB = model.DOB,
                Gender = model.Gender,
                Address = model.Address,
                Created_At = DateTime.UtcNow,
                PhoneNumber = model.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                isFirstTime=true,

                // Set other properties as needed
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            try
            {
                subscriptionRepository.AddAccountSubscription(user.Id);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            if (result.Succeeded)
            {
                // User registration successful
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Build the confirmation link
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                // Send the verification email
                await SendVerificationEmail(user.Email, confirmationLink);
                return Ok("Create success");
            }
            else
            {
                // User registration failed
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel) {

            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }
            return Unauthorized();
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                // Invalid user ID or token
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // User not found
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                user.Verified_At = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                return Ok();
            }
            else
            {
                // Email confirmation failed
                return BadRequest(result.Errors);
            }
        }
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
        private async Task SendVerificationEmail(string email, string confirmationLink)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Email Verification";

            var textBody = $"Thank you for registering! Please click the following link to confirm your email: {confirmationLink}";

            // Create the HTML body with the confirmation link
            var htmlBody = $"<p>Thank you for registering! Please click <a href=\"{confirmationLink}\">here</a> to confirm your email.</p>";

            // Create a multipart/alternative message body to support both plain text and HTML
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = textBody;
            bodyBuilder.HtmlBody = htmlBody;

            message.Body = bodyBuilder.ToMessageBody();
            // dùng SmtpClient của MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                await smtp.SendAsync(message);
            }
            catch (Exception ex)
            {
                // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await message.WriteToAsync(emailsavefile);


            }

            smtp.Disconnect(true);


        }

        [HttpPut]
        [Route("EditProfile")]
        public async Task<IActionResult> EditProfile(Guid id, [FromBody] UpdateAccountModel updateAccountModel)
        {
            var msg = "";
            try
            {
                var existed = await repository.GetUserById(id);


                existed.Address = updateAccountModel.Address;
                existed.DOB = updateAccountModel.DOB;
                existed.Gender = updateAccountModel.Gender;
                existed.PhoneNumber = updateAccountModel.PhoneNumber;


                repository.UpdateProfile(existed);
                msg = "Update success";
                return Ok(msg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Route("CheckLoginFirstTime/{accountId}")]
        public async Task<IActionResult> UpdateFirstTimeLogin(Guid accountId)
        {
            var account= await repository.GetUserById(accountId);
            if(account.isFirstTime ==true)
            {
                account.isFirstTime = false;
            }
            context.Accounts.Update(account);
            await context.SaveChangesAsync();
            return Ok(account);
        }

        [HttpGet]
        [Route("GetAccountDetail/{id}")]
        public async Task<IActionResult> GetAccountDetail([FromRoute]Guid id)
        {
            var account = new Account();
            try
            {
                account = await repository.GetUserById(id);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(account);
        }
        private string GenerateJwtToken(Account user)
        {
            string subname = subscriptionRepository.GetSubscriptionName(user.Id );
            var claims = new[]
            {
                                       new Claim("Id", user.Id.ToString()),
                                       new Claim("isFirstTime", user.isFirstTime.ToString(),ClaimValueTypes.Boolean),
                                       new Claim("Subscription Name", subname),


                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost]
        [Route("AddAccountGenre")]
        public async Task<IActionResult> AddAccountGenre(List<AccountGenreModel> model)
        {
            if (context.AccountGenre.Select(x => x.AccountId).Count() > 0)
            {
                var accountIds = model.Select(item => item.AccountId).ToList();
                List<AccountGenre> accountGenresToDelete = context.AccountGenre
                    .Where(ag => accountIds.Contains(ag.AccountId))
                    .ToList();

                context.AccountGenre.RemoveRange(accountGenresToDelete);
                context.SaveChanges();
            }


            foreach (var item in model)
                {
               
                    try
                    {
                        repository.AddAccountGenre(item);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }

            return Ok("Add success");
        }
        [HttpPut]
        [Route("UpdateAccountGenre")]
        public async Task<IActionResult> UpdateAccountGenre(AccountGenreModel sub)
        {
            try
            {
                await repository.UpdateAccountGenre(sub);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update");
            }
            return Ok("Update success");
        }
        [HttpPost]
        [Route("AddAccountArtist")]
        public async Task<IActionResult> AddAccountArtist(List<AccountArtistModel> model)
        {
            foreach (var item in model)
            {
                try
                {

                    repository.AddAccountArtist(item);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to add");
                }
            }
            return Ok("Add success");
        }
        [HttpPut]
        [Route("UpdateAccountArtist")]
        public async Task<IActionResult> UpdateAccountArtist(AccountArtistModel sub)
        {
            try
            {
                await repository.UpdateAccountArtist(sub);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update");
            }
            return Ok("Update success");
        }
    }
}
