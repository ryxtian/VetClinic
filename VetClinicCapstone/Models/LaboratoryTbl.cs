using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("LaboratoryTbl")]
    public partial class LaboratoryTbl
    {
        [Key]
        public long Laboratory_ID { get; set; }
        [Required]
        [StringLength(200)]
        [Unicode(false)]
        public string? LaboratoryName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? TestDate { get; set; }
        [StringLength(10)]
        public string? Status { get; set; }
        [StringLength(200)]
        [Unicode(false)]
        public string? LabFileName { get; set; }
        public long Consultation_ID { get; set; }

        [ForeignKey(nameof(Consultation_ID))]
        [InverseProperty(nameof(ConsultationTbl.LaboratoryTbls))]
        public virtual ConsultationTbl? Consultation { get; set; }
    }
}
