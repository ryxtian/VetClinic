namespace VetClinicCapstone.Models.ViewModel
{
	public class PetConsultViewModel
	{
		public List<ConsultationTbl> ConsultationTblsList { get; set; }//x
		public ConsultationTbl ConsultationTbls { get; set; }//x
		public PatientTbl PatientTbls { get; set; }
		public List<PrescriptionTbl> PrescriptionTbls { get; set; }
		public List<PrescriptionWithDoctorViewModel> PrescriptionWithDoctorViewModels { get; set; }
		public List<PrescriptionDetailTbl> PrescriptionDetailTbls { get; set; }
		public List<LaboratoryTbl> LaboratoryTbls { get; set; }
        public List<TreatmentTbl> TreatmentTbls { get; set; }
        public List<ConsultationWithDoctorViewModel> ConsultationWithDoctorViewModels { get; set; }
	}
}
