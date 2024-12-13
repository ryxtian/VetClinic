using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using VetClinicCapstone.Models;
using VetClinicCapstone.Models.ViewModel;
using VetClinicCapstone.Repository;
using VetClinicCapstone.EmailNotication;
using System;

namespace VetClinicCapstone.Controllers.Security
{
	[Authorize(Policy = "DoctorOrStaffOnly", AuthenticationSchemes = "AdminScheme")]
	public class UserAccount : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<UserTbl> _userRepo;
		private readonly IRepository<OwnerTbl> _ownerRepo;
		private readonly IEmailService _emailService;
		public UserAccount(IRepository<UserTbl> userRepo, ApplicationDbContext context, IRepository<OwnerTbl>  ownerRepo,IEmailService emailService)
        {
            _userRepo = userRepo;
            _context = context;
			_ownerRepo = ownerRepo;
			_emailService = emailService;
		}
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var useracc = await(from user in _context.UserTbls
                                                 join owner in _context.OwnerTbls
                                                 on user.Owner_ID equals owner.Owner_ID
                                                 where user.IsDeleted == 0
                                                 select new UserAndOwnerViewModel
                                                 {
                                                     OwnerDetail = owner,
                                                     UserDetail = user,

                                                 }).ToListAsync();
            return View("../Security/UserAccount/List",useracc);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(long ID)
        {
            var useracc = await _userRepo.GetByIDAsync(ID);
            if (useracc != null)
            {
                useracc.IsDeleted = 1;
                await _userRepo.SaveChangesAsync();
                return RedirectToAction("Index", "UserAccount");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long ID)
        {
            var user = await _userRepo.GetByIDAsync(ID);
            if (user != null)
            {
                var details = new UserTbl()
                {
                    Password = user.Password,
                    ConfirmPassword = user.ConfirmPassword,
                    Roles = user.Roles,
                };
                return View("../Security/UserAccount/Edit", details);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserTbl model, long Guid_id)
        {
            var message = "";
            var isSuccess = false;
            try
            {
                var user = await _userRepo.GetByIDAsync(Guid_id);
                if (user != null)
                {
                    user.Password = model.Password;
                    user.Roles = model.Roles;

                    _userRepo.Update(user);

                    isSuccess = true;
                    message = "Successfully saved!";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                // Handle exception appropriately
                // You might want to log the exception or return a different response based on your application's requirements
            }

            return Json(new { success = isSuccess, message = message });
        }
		public async Task<IActionResult> IsActivated(long ID)
		{
			var message = "";
			var title = "";
			var IsSuccess = false;

			try
			{
				var user = await (from u in _context.UserTbls
								  join o in _context.OwnerTbls
								  on u.Owner_ID equals o.Owner_ID
								  where u.User_ID == ID
								  select new
								  {
									  User = u,
									  Owner = o
								  }).FirstOrDefaultAsync();

				if (user != null)
				{
					if (user.User.IsActivated == 1)
					{
						IsSuccess = true;
						title = "Okay";
						message = "User is already activated.";
					}
					else
					{
						// Activate the user
						user.User.IsActivated = 1;
						_context.Update(user.User);
						await _context.SaveChangesAsync(); 

						IsSuccess = true;
						title = "Success";
						message = "User activated successfully.";

					
						string toEmail = user.Owner.Email; 
						string toName = user.Owner.Firstname; 
						string date = DateTime.Now.ToString("MMMM dd, yyyy");
						string time = DateTime.Now.ToString("hh:mm tt"); 

						string subject = "Your account is now activated";
						string emailMessage = $@"
Dear {toName},

We are pleased to inform you that your account has been successfully activated as of {date} at {time}. You can now log in and access all the features of our platform.

If you encounter any issues or need further assistance, please feel free to reach out to us.

Thank you for being a valued member of our community.

Best regards,

Urban Vet Animal Clinic
09676599591
urbanvetanimalclinic@gmail.com
urvanvetclinic.com
                ";

						// Send the email
						await _emailService.SendEmailAsync(toEmail, subject, emailMessage);
					}
				}
				else
				{
					title = "Error";
					message = "User not found.";
				}
			}
			catch (Exception ex)
			{
				title = "Error";
				message = "An error occurred while activating the user.";
				// Log the exception
				Console.WriteLine(ex.Message);
			}

			return Json(new { success = IsSuccess, title = title, message = message });
		}
		public async Task<IActionResult> IsDeactivated(long ID)
		{
			var message = "";
			var title = "";
			var IsSuccess = false;

			try
			{
				var userDetails = await (from u in _context.UserTbls
										 join o in _context.OwnerTbls
										 on u.Owner_ID equals o.Owner_ID
										 where u.User_ID == ID
										 select new
										 {
											 User = u,
											 Owner = o
										 }).FirstOrDefaultAsync();

				if (userDetails != null)
				{
					if (userDetails.User.IsActivated == 0)
					{
						IsSuccess = true;
						title = "Okay";
						message = "User is already deactivated.";
					}
					else
					{
						userDetails.User.IsActivated = 0;

						// Update the user entity in the context
						_context.UserTbls.Update(userDetails.User);
						await _context.SaveChangesAsync();

						IsSuccess = true;
						title = "Success";
						message = "User deactivated successfully.";

						// Prepare and send the email notification
						string toEmail = userDetails.Owner.Email;
						string toName = userDetails.Owner.Firstname;

						string subject = "Your Account Has Been Deactivated";
						string emailMessage = $@"
Dear {toName},

We want to inform you that your account has been deactivated as of {DateTime.Now.ToString("MMMM dd, yyyy")} at {DateTime.Now.ToString("hh:mm tt")}. 

If you believe this is an error or if you wish to reactivate your account, please contact our support team.

Thank you for your attention.

Best regards,

Urban Vet Animal Clinic  
09676599591  
urbanvetanimalclinic@gmail.com  
urvanvetclinic.com
                ";

						// Send the email
						await _emailService.SendEmailAsync(toEmail, subject, emailMessage);
					}
				}
				else
				{
					title = "Error";
					message = "User not found.";
				}
			}
			catch (Exception ex)
			{
				title = "Error";
				message = "An error occurred while deactivating the user.";
				// Log the exception
				Console.WriteLine(ex.Message);
			}

			return Json(new { success = IsSuccess, title = title, message = message });
		}

	}
}
