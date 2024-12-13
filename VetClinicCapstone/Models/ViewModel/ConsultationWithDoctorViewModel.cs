namespace VetClinicCapstone.Models.ViewModel
{
	public class ConsultationWithDoctorViewModel
	{
		public AdminInfoTbl DoctorTbls { get; set; }
		public PatientTbl PatientTbls { get; set; }
		public ConsultationTbl ConsultationTbls { get; set; }
        public PrescriptionTbl PrescriptionTbls { get; set; }
		public ServiceTbl ServiceTbls { get; set; }
		public LaboratoryTbl LaboratoryTbls { get; set; }
    }
}
