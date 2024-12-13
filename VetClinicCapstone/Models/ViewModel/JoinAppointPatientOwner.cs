using VetClinicCapstone.Models;
namespace VetClinicCapstone.Models.ViewModel
{
	public class JoinAppointPatientOwner
	{
		public AppointmentTbl AppointmentTbls { get; set; }
		public PatientTbl PatientTbls { get; set; }
		public OwnerTbl OwnerTbls { get; set; }
		public ConsultationTbl ConsultationTbls { get; set; }//x
        public TreatmentTbl TreatmentTbls { get; set; }
		public ServiceTbl ServiceTbls { get; set; }
	}
}
