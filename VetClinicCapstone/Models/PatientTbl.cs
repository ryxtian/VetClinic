using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("PatientTbl")]
    public partial class PatientTbl
    {
        public PatientTbl()
        {
            AppointmentTbls = new HashSet<AppointmentTbl>();
            ConsultationTbls = new HashSet<ConsultationTbl>();
            PrescriptionTbls = new HashSet<PrescriptionTbl>();
        }

        [Key]
        public long Patient_ID { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? PatientName { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Breed { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? Species { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string? Status { get; set; }
		public short? IsDeleted { get; set; }
		public long? Owner_ID { get; set; }
        [Unicode(false)]
        public string? Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Birthday { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string? Sex { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? ColorMarking { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateAdmit { get; set; }
        [StringLength(200)]
        [Unicode(false)]
        public string? FileName { get; set; }

        [ForeignKey(nameof(Owner_ID))]
        [InverseProperty(nameof(OwnerTbl.PatientTbls))]
        public virtual OwnerTbl? Owner { get; set; }
        [InverseProperty(nameof(AppointmentTbl.Patient))]
        public virtual ICollection<AppointmentTbl> AppointmentTbls { get; set; }
        [InverseProperty(nameof(ConsultationTbl.Patient))]
        public virtual ICollection<ConsultationTbl> ConsultationTbls { get; set; }
        [InverseProperty(nameof(PrescriptionTbl.Patient))]
        public virtual ICollection<PrescriptionTbl> PrescriptionTbls { get; set; }
    }
}
