using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using VetClinicCapstone.Models;
using Newtonsoft.Json;
using MailKit.Net.Smtp;
using System.Security.Cryptography;
using VetClinicCapstone.Models.ViewModel;
using NuGet.Common;
using System.Text;
namespace VetClinicCapstone.Controllers.ForgotPassword
{
    public class ForgotPassword : Controller
    {

        private readonly ApplicationDbContext _context;
        public ForgotPassword(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View("../UserForgotPassword/Index");
        }
        [HttpPost]
        public async Task<IActionResult>FindEmail(OwnerTbl model)
        {
            var findemail = await _context.OwnerTbls.FirstOrDefaultAsync(e=>e.Email == model.Email);

            ViewBag.Email = findemail.Email;
            if (findemail==null)
            {
                TempData["EmailNotFound"] = "Invalid Email.";
            }
            else
            {
                



                TempData["EmailStored"] = findemail.Email;
                TempData["FirstnameStored"] = findemail.Firstname;
                TempData["LastnameStored"] = findemail.Lastname;


                string token = GenerateRandomToken();
                return RedirectToAction("SendCodeConfirm", new { token = token });
            }

            return View("../UserForgotPassword/Index");
        }
        [HttpGet]
        public IActionResult SendCodeConfirm()
        {
            return View("../UserForgotPassword/SendCodeConfirm");
        }
        [HttpPost]
        public IActionResult VerifyCode(OwnerTbl model)
        {
            var verificationCode = new Random().Next(100000, 999999).ToString();

            // Store the code and model in TempData for later use
            TempData["VerificationCode"] = verificationCode;
            TempData["Email"] = model.Email;




            // Send email with the verification code using your existing MailKit setup
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Urban Vet Animal Clinic", "urbanvetanimalclinic@gmail.com"));
            message.To.Add(new MailboxAddress(model.Firstname, model.Email));
            message.Subject = "Your Email Verification Code";
            message.Body = new TextPart("plain")
            {
                Text = $"Your verification code is: {verificationCode}"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("urbanvetanimalclinic@gmail.com", "yqpz rpcq dbnv zwiv");
                client.Send(message);
                client.Disconnect(true);
            }
            return View("../UserForgotPassword/VerifyCode");
        }
        [HttpPost]
        public IActionResult NewPassword(string code)
        {
            if (code == TempData["VerificationCode"]?.ToString())
            {
                var email = TempData["Email"]?.ToString();

                TempData["EmailVerified"] = email;
                return View("../UserForgotPassword/NewPassword");
            }
            else
            {

            }
         TempData["VerfiedCode"] = "You enter code is invalid.";
           
            return View("../UserForgotPassword/VerifyCode");
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserAndOwnerViewModel model)
        {
            // Find the Owner by email
            var owner = await _context.OwnerTbls.FirstOrDefaultAsync(o => o.Email == model.OwnerDetail.Email);

            if (owner != null)
            {
             
                var user = await _context.UserTbls.FirstOrDefaultAsync(u => u.Owner_ID == owner.Owner_ID);

                if (user != null)
                {
           
                    user.Password = PasswordHasher.HashPassword(model.UserDetail.Password);
                    user.ConfirmPassword = PasswordHasher.HashPassword(model.UserDetail.Password);
                    // Save changes
                    _context.Update(user);
                    await _context.SaveChangesAsync();


                    return RedirectToAction("index","Userlogin");
                }
                else
                {
     
                    ModelState.AddModelError("", "User not found.");
                }
            }
            else
            {
                // Handle case when the owner is not found
                ModelState.AddModelError("", "Owner not found.");
            }

            return View("../UserForgotPassword/NewPassword"); // or return with error message
        }


        private string GenerateRandomToken()
        {
            // Generate a 32-byte random number
            byte[] randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            // Convert to a Base64 string for a unique token
            return Convert.ToBase64String(randomNumber).Replace("/", "-").Replace("+", "_").TrimEnd('=');
        }
        public static class PasswordHasher
        {
            public static string HashPassword(string password)
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }
            }
        }
    }
}
