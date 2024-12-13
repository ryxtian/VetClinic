using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("AdminInfoTbl")]
    public partial class AdminInfoTbl
    {
        public AdminInfoTbl()
        {
            AdminTbls = new HashSet<AdminTbl>();
            PrescriptionTbls = new HashSet<PrescriptionTbl>();
        }

        [Key]
        public long AdminInfo_ID { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? Firstname { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Middlename { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? Lastname { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? Role { get; set; }
        [Required]
        [StringLength(20)]
        [Unicode(false)]
        public string? Phone { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Street { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string? Barangay { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string? City { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string? Province { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string? Sex { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Birthday { get; set; }

        [InverseProperty(nameof(AdminTbl.AdminInfo))]
        public virtual ICollection<AdminTbl> AdminTbls { get; set; }
        [InverseProperty(nameof(PrescriptionTbl.AdminInfo))]
        public virtual ICollection<PrescriptionTbl> PrescriptionTbls { get; set; }
    }
}
