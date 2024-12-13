using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("PrescriptionDetailTbl")]
    public partial class PrescriptionDetailTbl
    {
        [Key]
        public long Prescription_Detail_ID { get; set; }
        public long? Prescription_ID { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string? Medicine { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Dosage { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string? Frequency { get; set; }
        public string? Instruction { get; set; }

        [ForeignKey(nameof(Prescription_ID))]
        [InverseProperty(nameof(PrescriptionTbl.PrescriptionDetailTbls))]
        public virtual PrescriptionTbl? Prescription { get; set; }
    }
}
