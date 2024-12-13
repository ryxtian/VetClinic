using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicCapstone.Models;
using VetClinicCapstone.Models.ViewModel;
using VetClinicCapstone.Repository;

namespace VetClinicCapstone.Controllers.Security
{
	[Authorize(Policy = "DoctorOnly", AuthenticationSchemes = "AdminScheme")]
	public class StaffAccount : Controller
	{
		private readonly IRepository<AdminInfoTbl> _admininfoRepo;
		private readonly IRepository<AdminTbl> _adminRepo;
		private readonly ApplicationDbContext _context;
		public StaffAccount(IRepository<AdminInfoTbl> admininfoRepo, IRepository<AdminTbl> adminRepo,ApplicationDbContext context)
        {
            _admininfoRepo = admininfoRepo;
			_adminRepo = adminRepo;
			_context = context;
		}
        public IActionResult Index()
		{
			var joinedData = from admin in _adminRepo.GetAll()
							 join adminInfo in _admininfoRepo.GetAll()
							 on admin.AdminInfo_ID equals adminInfo.AdminInfo_ID
							 where admin.IsDeleted == 0
							 select new AdminAndAdminInfoViewModel
							 {
								 AdminTbls = admin,
								 AdminInfoTbls = adminInfo
							 };

			return View("../Security/StaffAccount/Index", joinedData);
		}
		public async Task<IActionResult>CreateAdmin(AdminAndAdminInfoViewModel model)
		{
			var message = "";
			var title = "";
			var IsSuccess = false;

			try
			{
				var adminInfo = new AdminInfoTbl()
				{
					Firstname = model.AdminInfoTbls.Firstname,
					Middlename = model.AdminInfoTbls.Middlename,
					Lastname = model.AdminInfoTbls.Lastname,
					Role = model.AdminInfoTbls.Role,
					Phone = model.AdminInfoTbls.Phone,
                    Street = model.AdminInfoTbls.Street,
                    Barangay = model.AdminInfoTbls.Barangay,
                    City = model.AdminInfoTbls.City,
                    Province = model.AdminInfoTbls.Province,
                    Sex = model.AdminInfoTbls.Sex,
                    Birthday = model.AdminInfoTbls.Birthday,

                };

					await _admininfoRepo.AddAsync(adminInfo);
					await _admininfoRepo.SaveChangesAsync();

				var admin = new AdminTbl()
				{
					Username = model.AdminTbls.Username,
					Password = model.AdminTbls.Password,
					AdminInfo_ID = adminInfo.AdminInfo_ID

				};

				await _adminRepo.AddAsync(admin);
				await _adminRepo.SaveChangesAsync();

				 message = "Successfully saved!";
				 title = "New Account Added!";
				 IsSuccess = true;
			}
			catch (Exception ex)
			{
				message = ex.Message;
			}
			return Json(new {message = message, title = title,success = IsSuccess });
		}
        [HttpPost]
        public async Task<IActionResult> UpdateAdmin(AdminInfoTbl updatedAdminInfo)
        {
            var message = "";
            var title = "Update Admin Info";
            var isSuccess = false;

            try
            {
                var adminInfo = await _admininfoRepo.GetByIDAsync(updatedAdminInfo.AdminInfo_ID);

                if (adminInfo != null)
                {
                    adminInfo.Firstname = updatedAdminInfo.Firstname;
                    adminInfo.Lastname = updatedAdminInfo.Lastname;
                    adminInfo.Role = updatedAdminInfo.Role;
                    adminInfo.Phone = updatedAdminInfo.Phone;
                    adminInfo.Street = updatedAdminInfo.Street;
                    adminInfo.Barangay = updatedAdminInfo.Barangay;
                    adminInfo.City = updatedAdminInfo.City;
                    adminInfo.Province = updatedAdminInfo.Province;
                    adminInfo.Sex = updatedAdminInfo.Sex;

                    _admininfoRepo.Update(adminInfo);
					await _admininfoRepo.SaveChangesAsync();

                    isSuccess = true;
                    message = "Admin information updated successfully.";
                }
                else
                {
                    message = "Admin not found.";
                }
            }
            catch (Exception ex)
            {
                message = $"An error occurred while updating the admin info: {ex.Message}";
            }

            return Json(new { message = message, title = title, success = isSuccess });
        }
		public async Task<IActionResult> RemoveAdmin(long id)
		{
			var message = "";
            var title = "Update Admin Info";
            var IsSuccess = false;
			try
			{
				var del = await _adminRepo.GetByIDAsync(id);
				if(del !=null)
				{
					del.IsDeleted = 1;
                    _adminRepo.Update(del);
                    await _admininfoRepo.SaveChangesAsync();
                }
                IsSuccess = true;
                message = "Admin information removed.";

            }
            catch (Exception ex)
            {
                message = $"An error occurred while updating the admin info: {ex.Message}";
            }

            return Json(new { message = message, title = title, success = IsSuccess });
        }
    }
}
