using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("TreatmentTbl")]
    public partial class TreatmentTbl
    {
        [Key]
        public long Treatment_ID { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string? TreatmentType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FollowUpCheckUp { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string? Notes { get; set; }
        public long? Consultation_ID { get; set; }
        public long? Prescription_ID { get; set; }

        [ForeignKey(nameof(Consultation_ID))]
        [InverseProperty(nameof(ConsultationTbl.TreatmentTbls))]
        public virtual ConsultationTbl? Consultation { get; set; }
    }
}
