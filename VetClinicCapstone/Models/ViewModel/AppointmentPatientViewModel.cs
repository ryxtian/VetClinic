using static VetClinicCapstone.Controllers.Appointment.AppointSchedule;

namespace VetClinicCapstone.Models.ViewModel
{
	public class AppointmentPatientViewModel
	{
		public AppointmentTbl Appointment { get; set; }
		public PatientTbl Patient { get; set; }
		public ConsultationTbl Consultation { get; set; }
		public ServiceTbl Service { get; set; }
		public List<ServiceTbl> ServiceList { get; set; }
		public List<OwnerTbl> OwnerList { get; set; }//x
		public List<PatientTbl> PatientList { get; set; }//x
		public List<AppointmentScheduleTbl> AppointmentScheduleTbls { get; set; }
		public List<AppointmentPatientViewModel> AppointmentPatientViewModelList { get; set; }
	}
}
