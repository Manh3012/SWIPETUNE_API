using MimeKit;
using Repository.Repo;
using MailKit.Security;
using Repository.Interface;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Sub_Model;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace SWIPTETUNE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountRepository repository = new AccountRepository();
        private readonly IConfiguration _configuration;
        private readonly SignInManager<Account> _signInManager;
        private readonly UserManager<Account> _userManager;
        private readonly MailSettings mailSettings;

        public AccountController(IConfiguration configuration,IAccountRepository accountRepository, SignInManager<Account> signInManager, UserManager<Account> userManager, IOptions<MailSettings> _mailSettings)
        {
            _configuration = configuration;
            repository = accountRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            mailSettings = _mailSettings.Value;
           
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterAccountModel model)
        {
            var user = new Account
            {
                UserName = model.Email,
                Email = model.Email,
                // Set other properties as needed
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // User registration successful
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Build the confirmation link
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                // Send the verification email
                await SendVerificationEmail(user.Email, confirmationLink);
                return Ok();
            }
            else
            {
                // User registration failed
                return BadRequest(result.Errors);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel) {

            var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                // Login successful
                return Ok();
            }
            else
            {
                // Login failed
                return Unauthorized();
            }
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
            // Email confirmed successfully
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
        [AllowAnonymous]
        [HttpGet("LoginWithGoogle")]
        public IActionResult LoginWithGoogle()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync();
            if (!authenticateResult.Succeeded)
            {
                // Handle failed authentication
                return Unauthorized();
            }

            // Successful authentication
            // Get user details from authenticateResult.Principal
            // ...

            return Ok();
        }


    }
}
