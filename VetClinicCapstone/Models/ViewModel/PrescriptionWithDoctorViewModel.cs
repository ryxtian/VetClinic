namespace VetClinicCapstone.Models.ViewModel
{
	public class PrescriptionWithDoctorViewModel
	{
		public PrescriptionTbl PrescriptionTbls { get; set; }
		public ConsultationTbl ConsultationTbls { get; set; }
        public AdminInfoTbl AdminInfoTbls { get; set; }
		public List<ConsultationTbl> ConsultationTblsList { get; set; }



        public PatientTbl PatientTbls { get; set; }
        public List<PrescriptionDetailTbl> PrescriptionDetailTbls { get; set; }

    }
}