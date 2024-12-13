using Microsoft.EntityFrameworkCore;
using VetClinicCapstone.Models;
using VetClinicCapstone.Models.ViewModel;

namespace VetClinicCapstone.ReportServices
{
    public class ReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<ReportViewModel> GenerateReport(string reportType)
        //{
        //    var now = DateTime.Now;
        //    DateTime startDate;
        //    DateTime endDate = now;

        //    switch (reportType.ToLower())
        //    {
        //        case "daily":
        //            startDate = now.Date;
        //            break;
        //        case "weekly":
        //            startDate = now.AddDays(-(int)now.DayOfWeek);
        //            break;
        //        case "monthly":
        //            startDate = new DateTime(now.Year, now.Month, 1);
        //            break;
        //        case "yearly":
        //            startDate = new DateTime(now.Year, 1, 1);
        //            break;
        //        default:
        //            throw new ArgumentException("Invalid report type");
        //    }

        //    var patients = await (from p in _context.PatientTbls
        //                               join c in _context.ConsultationTbls on p.Patient_ID equals c.Patient_ID
        //                          join o in _context.OwnerTbls on p.Owner_ID equals o.Owner_ID
        //                          where c.ConsultationDate >= startDate && c.ConsultationDate <= endDate
        //                          select new JoinPatientOwnerViewModel
        //                          {
        //                              ConsultationTbls= c,
        //                              PatientTbls = p,
        //                              OwnerTbls = o
        //                               }).ToListAsync();


        //    var reportViewModel = new ReportViewModel
        //    {
        //        TotalPatients = patients.Count,
        //        ReportDate = now,
        //        ReportType = reportType,
        //        StartDate = startDate,
        //        EndDate = endDate,
        //        JoinPatientOwnerViewModels = patients
        //    };

        //    return reportViewModel;
        //}
        public async Task<ReportViewModel> GenerateReport(string reportType, DateTime? startDate, DateTime? endDate)
        {
            var now = DateTime.Now;
            startDate ??= now.Date;
            endDate ??= now;


            switch (reportType.ToLower())
            {
                case "patient":
                    if (startDate == null || endDate == null)
                    {
                        startDate = now.Date;
                        endDate = now.Date;
                    }
                    break;
               
                default:
                    throw new ArgumentException("Invalid report type");
            }



            var patients = await (from p in _context.PatientTbls
                                  join c in _context.ConsultationTbls on p.Patient_ID equals c.Patient_ID
                                  join o in _context.OwnerTbls on p.Owner_ID equals o.Owner_ID
                                  where c.ConsultationDate >= startDate && c.ConsultationDate <= endDate
                                  group new { p, c, o } by new { o.Owner_ID, o.Firstname,o.Lastname } into g
                                  select new JoinPatientOwnerViewModel
                                  {
                                      OwnerID = g.Key.Owner_ID, // Keep Owner_ID for grouping
                                      OwnerName = $"{g.Key.Firstname} {g.Key.Lastname}", // Format Name
                                      Patients = g.Select(x => new PatientDetails
                                      {
                                          PatientName = x.p.PatientName,
                                          Species = x.p.Species,
                                          Breed = x.p.Breed,
                                          ConsultationDate = x.c.ConsultationDate
                                      }).ToList()
                                  }).ToListAsync();

            var reportViewModel = new ReportViewModel
            {
                TotalPatients = patients.Sum(g => g.Patients.Count),
                ReportDate = now,
                ReportType = reportType,
                JoinPatientOwnerViewModels = patients,
                StartDate = startDate.Value,
                EndDate = endDate.Value
            };

            return reportViewModel;



            //var patients = await (from p in _context.PatientTbls
            //                      join c in _context.ConsultationTbls on p.Patient_ID equals c.Patient_ID
            //                      join o in _context.OwnerTbls on p.Owner_ID equals o.Owner_ID
            //                      where c.ConsultationDate >= startDate && c.ConsultationDate <= endDate
            //                      select new JoinPatientOwnerViewModel
            //                      {
            //                          ConsultationTbls = c,
            //                          PatientTbls = p,
            //                          OwnerTbls = o
            //                      }).ToListAsync();

            //var reportViewModel = new ReportViewModel
            //{
            //    TotalPatients = patients.Count,
            //    ReportDate = now,
            //    ReportType = reportType,
            //    JoinPatientOwnerViewModels = patients,
            //    StartDate = startDate.Value,
            //    EndDate = endDate.Value
            //};

            //return reportViewModel;
        }

        public async Task<ReporAppointmentViewModel> GenerateAppointmentReport(string reportType, DateTime? startDate, DateTime? endDate)
        {
            var now = DateTime.Now;
            startDate ??= now.Date;
            endDate ??= now;


            switch (reportType.ToLower())
            {
                case "appointment":
                    if (startDate == null || endDate == null)
                    {
                        startDate = now.Date;
                        endDate = now.Date;
                    }
                    break;

                default:
                    throw new ArgumentException("Invalid report type");
            }
            var appointments = await (from p in _context.PatientTbls
                                  join c in _context.ConsultationTbls on p.Patient_ID equals c.Patient_ID
                                  join o in _context.OwnerTbls on p.Owner_ID equals o.Owner_ID
                                  join a in _context.AppointmentTbls on p.Owner_ID equals a.Owner_ID
								  join s in _context.ServiceTbls on a.Service_ID equals s.Service_ID
									  where a.Date >= startDate && a.Date <= endDate && a.Status=="Completed"
                                  select new JoinAppointPatientOwner
                                  {
                                      ConsultationTbls = c,
                                      PatientTbls = p,
                                      OwnerTbls = o,
                                      AppointmentTbls = a,
									  ServiceTbls = s,
								  }).ToListAsync();

            var reportViewModel = new ReporAppointmentViewModel
            {
                TotalAppointments = appointments.Count,
                ReportDate = now,
                ReportType = reportType,
                JoinAppointPatientOwnerViewModels = appointments,
                StartDate = startDate.Value,
                EndDate = endDate.Value
            };

            return reportViewModel;
        }
		public async Task<ReportConsultationViewModel> GenerateConsultationReport(string reportType, DateTime? startDate, DateTime? endDate)
		{
			var now = DateTime.Now;
			startDate ??= now.Date;
			endDate ??= now;


			switch (reportType.ToLower())
			{
				case "consultation":
					if (startDate == null || endDate == null)
					{
						startDate = now.Date;
						endDate = now.Date;
					}
					break;

				default:
					throw new ArgumentException("Invalid report type");
			}
            //var consultations = await (from p in _context.PatientTbls
            //                           join c in _context.ConsultationTbls on p.Patient_ID equals c.Patient_ID
            //                           join o in _context.OwnerTbls on p.Owner_ID equals o.Owner_ID
            //                           join t in _context.TreatmentTbls on c.Consultation_ID equals t.Consultation_ID
            //                           join s in _context.ServiceTbls on c.Service_ID equals s.Service_ID

            //                           where c.ConsultationDate >= startDate && c.ConsultationDate <= endDate
            //                           select new JoinAppointPatientOwner
            //                           {
            //                               ConsultationTbls = c,
            //                               PatientTbls = p,
            //                               OwnerTbls = o,
            //                               TreatmentTbls = t,
            //                               ServiceTbls = s
            //                           }).ToListAsync();
            var consultations = await (from p in _context.PatientTbls
                                       join c in _context.ConsultationTbls on p.Patient_ID equals c.Patient_ID
                                       join o in _context.OwnerTbls on p.Owner_ID equals o.Owner_ID
                                       join t in _context.TreatmentTbls on c.Consultation_ID equals t.Consultation_ID into treatmentGroup
                                       from t in treatmentGroup.DefaultIfEmpty()  // This represents a left join
                                       join s in _context.ServiceTbls on c.Service_ID equals s.Service_ID
                                       where c.ConsultationDate >= startDate && c.ConsultationDate <= endDate

                                       select new JoinAppointPatientOwner
                                       {
                                           PatientTbls = p,
                                           ConsultationTbls = c,
                                           OwnerTbls = o,
                                           TreatmentTbls = t, // This will be null if there's no matching record
                                           ServiceTbls = s
                                       }).ToListAsync();

            var reportViewModel = new ReportConsultationViewModel
            {
				TotalConsulted = consultations.Count,
				ReportDate = now,
				ReportType = reportType,
				JoinAppointPatientOwnerViewModels = consultations,
				StartDate = startDate.Value,
				EndDate = endDate.Value
			};

			return reportViewModel;
		}
	}

}
