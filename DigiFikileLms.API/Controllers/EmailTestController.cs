using Microsoft.AspNetCore.Mvc;
using DigiFikileLms.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;

namespace DigiFikileLms.API.Controllers
{
    [ApiController]
    [Route("api/test-email")]
   //[AllowAnonymous]
    public class EmailTestController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailTestController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost("send")]
        public async Task<IActionResult> SendTestEmail([FromQuery] string email)
        {
            await _emailService.SendEmailAsync(
                email,
                "DigiFikile Email Test",
                "<h1>Email service working</h1><p>This is a DigiFikile LMS test email.</p>"
            );

            return Ok(new
            {
                message = "Test email sent.",
                email
            });
        }

        //to get otp
    [AllowAnonymous]
    [HttpPost("otp")]
    public async Task<IActionResult> SendOtp(
        [FromQuery] string email)
    {
        var otp = System.Security.Cryptography.RandomNumberGenerator
            .GetInt32(100000, 1000000)
            .ToString();

        await _emailService.SendOtpEmailAsync(
            email,
            otp,
            "Test User",
            10
        );

        return Ok(new
        {
            message = "OTP email sent successfully.",
            email,

            // Development testing only
            otp
        });
    }

//invitation
        [AllowAnonymous]
        [HttpPost("invitation")]
        public async Task<IActionResult> SendInvitation(
            [FromQuery] string email)
        {
            var invitationLink =
                "http://localhost:5173/create-password?token=test-token";

            await _emailService.SendAccountInvitationAsync(
                email,
                "Test User",
                invitationLink
            );

            return Ok(new
            {
                message = "Account invitation sent successfully.",
                email
            });
        }
        }
}