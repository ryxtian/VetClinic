namespace VetClinicCapstone.Models.ViewModel
{
    public class ReporAppointmentViewModel
    {
        //public int TotalPatients { get; set; }
        //public DateTime ReportDate { get; set; }
        //public string ReportType { get; set; } // Daily, Weekly, Monthly, Yearly
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        //public List<JoinPatientOwnerViewModel> JoinPatientOwnerViewModels { get; set; }

        public int TotalAppointments { get; set; }
        public DateTime ReportDate { get; set; }
        public string ReportType { get; set; } // Daily, Weekly, Monthly, Yearly
        public List<JoinAppointPatientOwner> JoinAppointPatientOwnerViewModels { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
