using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("ConsultationTbl")]
    public partial class ConsultationTbl
    {
        public ConsultationTbl()
        {
            LaboratoryTbls = new HashSet<LaboratoryTbl>();
            TreatmentTbls = new HashSet<TreatmentTbl>();
        }

        [Key]
        public long Consultation_ID { get; set; }
        [Required]
        [StringLength(10)]
        [Unicode(false)]
        public string? Temparature { get; set; }
        [Required]
        [StringLength(10)]
        [Unicode(false)]
        public string? Weight { get; set; }
        [Required]
        [StringLength(20)]
        [Unicode(false)]
        public string? RepiratoryRate { get; set; }
        [Required]
        [StringLength(20)]
        [Unicode(false)]
        public string? HeartRate { get; set; }
        public long? Patient_ID { get; set; }
        [Unicode(false)]
        public string? ClinicalSign { get; set; }
        [Unicode(false)]
        public string? Diagnosis { get; set; }
		public long? Service_ID { get; set; }

		public long AdminInfo_ID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ConsultationDate { get; set; }

        [ForeignKey(nameof(Patient_ID))]
        [InverseProperty(nameof(PatientTbl.ConsultationTbls))]
        public virtual PatientTbl? Patient { get; set; }
        [InverseProperty(nameof(LaboratoryTbl.Consultation))]
        public virtual ICollection<LaboratoryTbl> LaboratoryTbls { get; set; }
        [InverseProperty(nameof(TreatmentTbl.Consultation))]
        public virtual ICollection<TreatmentTbl> TreatmentTbls { get; set; }
    }
}
