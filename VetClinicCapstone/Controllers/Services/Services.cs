using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VetClinicCapstone.Models;
using VetClinicCapstone.Repository;

namespace VetClinicCapstone.Controllers.Services
{
	[Authorize(Policy = "DoctorOrStaffOnly", AuthenticationSchemes = "AdminScheme")]
	public class Services : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IRepository<ServiceTbl> _serviceRepo;
		public Services(ApplicationDbContext context, IRepository<ServiceTbl> serviceRepo)
		{
			_context = context;
			_serviceRepo = serviceRepo;
		}
		public IActionResult Index()
		{
			var services = _serviceRepo.GetAll();
			return View("../Services/Index", services);
		}
		[HttpPost]
		public async Task<IActionResult> AddServices(ServiceTbl model)
		{
			var message = "";
			var title = "";
			var isSuccess = false;
			try
			{
				var AdminID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				var addServices = new ServiceTbl()
				{
					ServiceName = model.ServiceName,
					ServicePrice = model.ServicePrice,
					CreatedBy = Convert.ToInt32(AdminID),
					CreatedDate = DateTime.Now
				};
				await _serviceRepo.AddAsync(addServices);
				await _serviceRepo.SaveChangesAsync();

				isSuccess = true;
				message = "Successfully saved!";
				title = "Service Added!";
			}
			catch (Exception ex)
			{
				message = ex.Message;
			}
			
			return Json(new {message=message,title = title,success= isSuccess });
		}
		[HttpPost]
		public async Task<IActionResult> UpdateService(ServiceTbl model)
		{
			var message = "";
			var title = "";
			var isSuccess = false;

			try
			{
				var AdminID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				var getServiceID = await _serviceRepo.GetByIDAsync(model.Service_ID);
				if (getServiceID != null)
				{
					getServiceID.ServiceName = model.ServiceName;
					getServiceID.ServicePrice = model.ServicePrice;
					getServiceID.UpdatedBy = Convert.ToInt32(AdminID);
					getServiceID.UpdateDate = DateTime.Now;

					_serviceRepo.Update(getServiceID);
					await _serviceRepo.SaveChangesAsync();

					isSuccess = true;
					message = "Successfully Updated!";
					title = "Service Updated!";

				};
			}
			catch (Exception ex)
			{

				message = ex.Message;
			}
			

			return Json(new {title=title, message=message,success =isSuccess });
		}
		[HttpGet]
		public async Task<IActionResult> DeleteService(long ID)
		{
			var AdminID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var getServiceID = await _serviceRepo.GetByIDAsync(ID);
			if (getServiceID != null)
			{
				getServiceID.IsDeleted = 1;
				getServiceID.DeletedBy = Convert.ToInt32(AdminID);
				getServiceID.DeletedAt = DateTime.Now;

				_serviceRepo.Update(getServiceID);
				await _serviceRepo.SaveChangesAsync();

				return RedirectToAction("Index");

			};

			return View("../Services/Index");
		}
		[HttpGet]
		public async Task<IActionResult> ActiveService(long ID)
		{
			var AdminID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var getServiceID = await _serviceRepo.GetByIDAsync(ID);
			if (getServiceID != null)
			{
				getServiceID.IsDeleted = 0;
				getServiceID.UpdatedBy = Convert.ToInt32(AdminID);
				getServiceID.UpdateDate = DateTime.Now;

				_serviceRepo.Update(getServiceID);
				await _serviceRepo.SaveChangesAsync();

				return RedirectToAction("Index");

			};

			return View("../Services/Index");
		}
	}
}
