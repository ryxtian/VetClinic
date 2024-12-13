using System.Linq.Expressions;
using VetClinicCapstone.Models;

namespace VetClinicCapstone.Repository
{
    public interface IRepository<T>
    {
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

		Task<T> GetByEmail(string Email);
		Task<T> GetByIDAsync(long ID);
        IEnumerable<T> GetAll();
		Task AddAsync(T entity);
		void Update(T entity);
		//Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
		Task SaveChangesAsync();
		Task AddAppointAsync(AppointmentTbl appoint);
		Task AddOwnerAsync(OwnerTbl owner);
		Task AddUserAsync(UserTbl user);

		Task AddPatientAsync(PatientTbl patient);
	}
}
