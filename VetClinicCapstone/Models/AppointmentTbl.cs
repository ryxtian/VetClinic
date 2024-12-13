using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("AppointmentTbl")]
    public partial class AppointmentTbl
    {
        [Key]
        public long Appointment_ID { get; set; }
        public long? Patient_ID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        [Required]
        [StringLength(10)]
        [Unicode(false)]
        public string? Time { get; set; }
        public short? IsDeleted { get; set; }
        [Required]
        [StringLength(20)]
        [Unicode(false)]
        public string? Status { get; set; }
        public long Owner_ID { get; set; }
        public bool IsViewed { get; set; }
		public long? Service_ID { get; set; }

		[ForeignKey(nameof(Owner_ID))]
        [InverseProperty(nameof(OwnerTbl.AppointmentTbls))]
        public virtual OwnerTbl? Owner { get; set; }
        [ForeignKey(nameof(Patient_ID))]
        [InverseProperty(nameof(PatientTbl.AppointmentTbls))]
        public virtual PatientTbl? Patient { get; set; }
    }
}
