using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("PrescriptionTbl")]
    public partial class PrescriptionTbl
    {
        public PrescriptionTbl()
        {
            PrescriptionDetailTbls = new HashSet<PrescriptionDetailTbl>();
        }

        [Key]
        public long Prescription_ID { get; set; }
        public long? Patient_ID { get; set; }
        public long? AdminInfo_ID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DatePrescribed { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string? Disease { get; set; }
        public long? Consultation_ID { get; set; }

        [ForeignKey(nameof(AdminInfo_ID))]
        [InverseProperty(nameof(AdminInfoTbl.PrescriptionTbls))]
        public virtual AdminInfoTbl? AdminInfo { get; set; }
        [ForeignKey(nameof(Patient_ID))]
        [InverseProperty(nameof(PatientTbl.PrescriptionTbls))]
        public virtual PatientTbl? Patient { get; set; }
        [InverseProperty(nameof(PrescriptionDetailTbl.Prescription))]
        public virtual ICollection<PrescriptionDetailTbl> PrescriptionDetailTbls { get; set; }
    }
}
