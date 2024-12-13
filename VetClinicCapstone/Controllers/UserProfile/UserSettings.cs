using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VetClinicCapstone.Models;
using VetClinicCapstone.Models.ViewModel;
using Newtonsoft.Json;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;

namespace VetClinicCapstone.Controllers.UserProfile
{
    [Authorize(Policy = "UserOnly", AuthenticationSchemes = "UserScheme")]
    public class UserSettings : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserSettings(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(userId, out int userIdInt))
            {
                return BadRequest("Invalid user ID.");
            }

            var userProfile = await (from user in _context.UserTbls
                                     join owner in _context.OwnerTbls
                                     on user.Owner_ID equals owner.Owner_ID
                                     where user.Owner_ID == userIdInt
                                     select new UserAndOwnerViewModel
                                     {
                                         UserDetail = user,
                                         OwnerDetail = owner
                                     })
                                    .FirstOrDefaultAsync();

            if (userProfile == null)
            {
                return NotFound();
            }

            return View("../User/UserSettings/index", userProfile); 
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
        public async Task<IActionResult> UpdateEmailSettings(UserAndOwnerViewModel model)
        {
            var owner = await _context.OwnerTbls.FindAsync(model.OwnerDetail.Owner_ID);

            if (owner != null)
            {
                if (owner.Email != model.OwnerDetail.Email)
                {
                    var verificationCode = new Random().Next(100000, 999999).ToString();

                    TempData["NewEmail"] = model.OwnerDetail.Email;
                    TempData["VerificationCode"] = verificationCode;
                    TempData["Owner_ID"] = model.OwnerDetail.Owner_ID.ToString();

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Urban Vet Animal Clinic", "tikoyryan@gmail.com"));
                    message.To.Add(new MailboxAddress(model.OwnerDetail.Firstname, model.OwnerDetail.Email));
                    message.Subject = "Email Verification Code";
                    message.Body = new TextPart("plain")
                    {
                        Text = $"Your verification code is: {verificationCode}"
                    };

                    using (var client = new SmtpClient())
                    {
                        client.Connect("smtp.gmail.com", 587, false);
                        client.Authenticate("tikoyryan@gmail.com", "wowl isxr ucpu vuka");
                        client.Send(message);
                        client.Disconnect(true);
                    }

                    return RedirectToAction("VerifyEmailCode");
                }

                if (owner.Phone != model.OwnerDetail.Phone)
                {
                    owner.Phone = model.OwnerDetail.Phone;
                    _context.OwnerTbls.Update(owner);
                    await _context.SaveChangesAsync();

                    TempData["UpdateSettngs"] = "Phone number updated successfully.";
                    return RedirectToAction("Index");
                }
            }

            TempData["UpdateError"] = "Owner not found.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult VerifyEmailCode()
        {
            return View("../User/UserSettings/VerifyEmailCode");
        }
        [HttpPost]
        public async Task<IActionResult> VerifyEmailCode(string verificationCode)
        {
            var storedCode = TempData["VerificationCode"]?.ToString();
            var newEmail = TempData["NewEmail"]?.ToString();
            var ownerId = Convert.ToInt64(TempData["Owner_ID"]);

            if (storedCode == verificationCode)
            {
                var owner = await _context.OwnerTbls.FindAsync(ownerId);
                if (owner != null && !string.IsNullOrEmpty(newEmail))
                {
                    owner.Email = newEmail;
                    _context.OwnerTbls.Update(owner);
                    await _context.SaveChangesAsync();

                    TempData["UpdateSettngs"] = "Email updated successfully.";
                    return RedirectToAction("Index");
                }
            }

            TempData["TokenInvalid"] = "Invalid verification code.";
            return RedirectToAction("VerifyEmailCode");
        }
        //[HttpPost]
        //public async Task<IActionResult> UpdateEmailSettings(UserAndOwnerViewModel model)
        //{
        //    var getEmail = await _context.OwnerTbls.FindAsync(model.OwnerDetail.Owner_ID);
        //    if (getEmail !=null)
        //    {
        //        getEmail.Email = model.OwnerDetail.Email;


        //        _context.OwnerTbls.Update(getEmail);
        //        await _context.SaveChangesAsync();

        //        return RedirectToAction("Index");
        //    }
        //    return RedirectToAction("Index");

        //}
        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserChangePassViewModel model)
        {
            var user = await _context.UserTbls.FindAsync(model.User_ID);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            var hashedCurrentPassword = PasswordHasher.HashPassword(model.CurrentPassword);
            if (user.Password != hashedCurrentPassword)
            {
                TempData["Error"] = "Current password is incorrect.";
                return RedirectToAction("Index");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["NewAndConPass"] = "New Password and Confirm New Password do not match.";
                return RedirectToAction("Index");
            }
        

            user.ConfirmPassword = PasswordHasher.HashPassword(model.ConfirmPassword);
            user.Password = PasswordHasher.HashPassword(model.NewPassword);

            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("Index");
        }

    }
}
