using Microsoft.AspNetCore.Mvc;
using System.Text;
using VetClinicCapstone.Models.ViewModel;
using VetClinicCapstone.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Razor.TagHelpers;
using VetClinicCapstone.Repository;
using MimeKit;
using Newtonsoft.Json;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Controllers.Register
{
    public class UserRegistration : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<UserTbl> _userRepo;
        public UserRegistration(ApplicationDbContext context,IRepository<UserTbl> userRepo)
        {
            _context = context;
            _userRepo = userRepo;
        }
        public IActionResult Index()
        {
            return View("../UserRegister/Index");
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

        [HttpPost]
        public async Task<IActionResult> Register(UserAndOwnerViewModel model)
        {
            // Check if the email is already in the database
            var existingOwner = await _context.OwnerTbls
                                    .FirstOrDefaultAsync(o => o.Email == model.OwnerDetail.Email);

            if (existingOwner != null)
            {
                // If the email already exists, add a model error and return the view
                TempData["EmailAlreadyUse"] = "Email is already in use. Please use a different email.";
                return RedirectToAction("Index");
            }

			// Generate a random verification code
			var verificationCode = new Random().Next(100000, 999999).ToString();

            // Store the code and model in TempData for later use
            TempData["VerificationCode"] = verificationCode;
            TempData["UserAndOwnerModel"] = JsonConvert.SerializeObject(model);

            // Send email with the verification code using your existing MailKit setup
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Urban Vet Animal Clinic", "urbanvetanimalclinic@gmail.com"));
            message.To.Add(new MailboxAddress(model.OwnerDetail.Firstname, model.OwnerDetail.Email));
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
            return RedirectToAction("Verify");
        }

		// If we got this far, something failed, redisplay form

		[HttpGet]
		public IActionResult Verify()
		{
			// Generate a random token
			string token = GenerateRandomToken();

			// Redirect to the Verify action with the token as a URL parameter
			return RedirectToAction("VerifyEmail", new { token = token });
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
		public IActionResult VerifyEmail()
        {
            return View("../UserRegister/Verify");
        }
		[HttpPost]
        public async Task<IActionResult> Verify(string code)
        {
            if (code == TempData["VerificationCode"]?.ToString())
            {
                if (code == null)
                {
                    TempData["TokenInvalid"] = "Please enter the verification code";
                    return RedirectToAction("VerifyEmail");
                }
                // Retrieve the model from TempData
                var model = JsonConvert.DeserializeObject<UserAndOwnerViewModel>(TempData["UserAndOwnerModel"].ToString());


                // Save owner and user details
                var owner = new OwnerTbl
                {
                    Firstname = model.OwnerDetail.Firstname,
                    Midname = model.OwnerDetail.Midname,
                    Lastname = model.OwnerDetail.Lastname,
                    SuffixName = model.OwnerDetail.SuffixName,
                    Email = model.OwnerDetail.Email,
                    Phone = model.OwnerDetail.Phone,
                    Province = model.OwnerDetail.Province,
                    City = model.OwnerDetail.City,
                    Baranggay = model.OwnerDetail.Baranggay,
                    Street = model.OwnerDetail.Street,
                    Sex = model.OwnerDetail.Sex,
                };
                await _userRepo.AddOwnerAsync(owner);
                await _userRepo.SaveChangesAsync();

                var user = new UserTbl
                {
                    Password = PasswordHasher.HashPassword(model.UserDetail.Password),
                    ConfirmPassword = PasswordHasher.HashPassword(model.UserDetail.ConfirmPassword),
                    DateRegistered = DateTime.Now,
                    Roles = "User",
                    Owner_ID = owner.Owner_ID
                };

                await _userRepo.AddUserAsync(user);
                await _userRepo.SaveChangesAsync();

                return RedirectToAction("Index", "UserLogin");
            }
            else
            {
                // If verification fails, return an error message
                TempData["TokenInvalid"] = "The code you entered is invalid. Please try again.";
                return RedirectToAction("VerifyEmail");
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> Register(UserAndOwnerViewModel model)
        //{

        //    var owner = new OwnerTbl
        //    {
        //        Firstname = model.OwnerDetail.Firstname,
        //        Midname = model.OwnerDetail.Midname,
        //        Lastname = model.OwnerDetail.Lastname,
        //        SuffixName = model.OwnerDetail.SuffixName,
        //        Email = model.OwnerDetail.Email,
        //        Phone = model.OwnerDetail.Phone,
        //        Province = model.OwnerDetail.Province,
        //        City = model.OwnerDetail.City,
        //        Baranggay = model.OwnerDetail.Baranggay,
        //        Street = model.OwnerDetail.Street,
        //        Sex = model.OwnerDetail.Sex,
        //        //CreatedAt = DateTime.Now,
        //    };
        //    await _userRepo.AddOwnerAsync(owner);
        //    await _userRepo.SaveChangesAsync();

        //    var user = new UserTbl
        //    {

        //        Password = PasswordHasher.HashPassword(model.UserDetail.Password),
        //        ConfirmPassword = PasswordHasher.HashPassword(model.UserDetail.ConfirmPassword),
        //        DateRegistered = DateTime.Now,
        //        Roles = "User",
        //        Owner_ID = owner.Owner_ID
        //    };



        //    await _userRepo.AddUserAsync(user);
        //    await _userRepo.SaveChangesAsync();

        //    return RedirectToAction("Index");
        //}
    }
}
