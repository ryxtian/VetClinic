namespace VetClinicCapstone.Models.ViewModel
{
	public class JoinPatientOwnerViewModel
	{
		public List<ConsultationTbl> ConsultationTblsList { get; set; }
		public List<PatientTbl> PatientTblsList { get; set; }
		public OwnerTbl OwnerTbls { get; set; }
        public PatientTbl PatientTbls { get; set; }
		public ConsultationTbl ConsultationTbls { get; set; }
		public List<ConsultationWithDoctorViewModel> ConsultationWithDoctorViewModels { get; set; }
		public List<PrescriptionWithDoctorViewModel> PrescriptionWithDoctorViewModels { get; set; }
		public List<PrescriptionDetailTbl> PrescriptionDetailTbls { get; set; }
        public List<LaboratoryTbl> LaboratoryTbls { get; set; }
        public List<PatientDetails> Patients { get; set; }
		public List<TreatmentTbl> TreatmentTbls { get; set; }
		public List<ServiceTbl> ServiceTbls { get; set; }
		public string OwnerName { get; set; } //x
        public long OwnerID { get; set; }//x
        public long PatientID { get; set; }//x

    }

  
    public class PatientDetails   //xx
    {
        public string PatientName { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        public DateTime ConsultationDate { get; set; }
    }
}
