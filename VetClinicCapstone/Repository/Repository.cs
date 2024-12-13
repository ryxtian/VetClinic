
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Linq.Expressions;
using VetClinicCapstone.Models;

namespace VetClinicCapstone.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }
		public async Task AddAsync(T entity)
        {
           await _context.Set<T>().AddAsync(entity);
        }

		//public Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
		//{
		//    return _context.Set<T>().FirstOrDefaultAsync(predicate);
		//}
		public async Task SaveChangesAsync()
		{
            await _context.SaveChangesAsync();
        }
        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public async Task<T> GetByIDAsync(long ID)
        {
			return await _context.Set<T>().FindAsync(ID);
		}
        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }
        public async Task<T> GetByEmail(string Email)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(e => EF.Property<string>(e, "Email") == Email);

        }


        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
		public async Task AddUserAsync(UserTbl user)
		{
			await _context.UserTbls.AddAsync(user);
		}
		public async Task AddOwnerAsync(OwnerTbl owner)
		{
			await _context.OwnerTbls.AddAsync(owner);
		}
		public async Task AddPatientAsync(PatientTbl patient)
		{
			await _context.PatientTbls.AddAsync(patient);
		}
		public async Task AddAppointAsync(AppointmentTbl appoint)
		{
			await _context.AppointmentTbls.AddAsync(appoint);
		}
	}
}
