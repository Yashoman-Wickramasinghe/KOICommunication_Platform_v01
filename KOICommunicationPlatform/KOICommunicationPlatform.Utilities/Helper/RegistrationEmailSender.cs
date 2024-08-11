using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Utilities.EmailSender;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Utilities.Helper
{
    public class RegistrationEmailSender : IRegistrationEmailSender
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailDispatcher _emailDispatcher;
        private readonly IConfiguration _configuration;

        public RegistrationEmailSender(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailDispatcher emailDispatcher,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _emailDispatcher = emailDispatcher;
            _configuration = configuration;

        }
        public void SendUserRegistrationEmail(ApplicationUser applicationUser, string userName)
        {
            try
            {
                _userManager.AddToRoleAsync(applicationUser, SD.Website_Client).GetAwaiter().GetResult();

                var applicationDomain = _configuration["EmailSettings:Domain"];

                // Generate password reset token
                var token = _userManager.GeneratePasswordResetTokenAsync(applicationUser).GetAwaiter().GetResult();

                // URL encode the token
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var resetPasswordUrl = $"https://{applicationDomain}/Identity/Account/ResetPassword?userId={applicationUser.Id}&code={encodedToken}";

                // Compose the email
                var subject = "Password Reset Request";

                var message = $@"
<div style='font-family: Arial, sans-serif; color: #333;'>
    <div style='background-color: #f7f7f7; padding: 20px;'>
        <img src='https://media.licdn.com/dms/image/D563DAQGz7eF5nYkI2w/image-scale_191_1128/0/1706753270420/kings_own_institute_cover?e=2147483647&v=beta&t=VUkOybxE5Cvah5a1ghXMaFsETiQNEIP2GNFXLzkb67o' alt='KOI Banner' style='width: 100%; height: auto;'>
    </div>
    <div style='padding: 20px; background-color: #ffffff;'>
        <h2 style='color: #11587d;'>Hi {userName},</h2>
        <p>We welcome you to the KOI Communication Platform. Please click the link below to reset your password:</p>
        <p style='text-align: left;'>
            <a href='{resetPasswordUrl}' target='_blank' style='display: inline-block; padding: 10px 20px; background-color: #11587d; color: #ffffff; text-decoration: none; border-radius: 5px;'>Reset Your Password</a>
        </p>
        <p>If you did not request a password reset, please ignore this email. If you have any questions or need further assistance, feel free to contact our support team.</p>
        <p style='color: #999;'>Best regards,<br>The KOI Team</p>
    </div>
</div>";
                _emailDispatcher.SendEmailAsync(applicationUser.Email, subject, message).GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }
    }
}
