using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("OwnerTbl")]
    public partial class OwnerTbl
    {
        public OwnerTbl()
        {
            AppointmentTbls = new HashSet<AppointmentTbl>();
            PatientTbls = new HashSet<PatientTbl>();
            UserTbls = new HashSet<UserTbl>();
        }

        [Key]
        public long Owner_ID { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? Firstname { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Midname { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? Lastname { get; set; }
        [StringLength(10)]
        public string? SuffixName { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? Province { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? City { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? Baranggay { get; set; }
        [Required]
        [StringLength(100)]
        public string? Street { get; set; }
        [Required]
        [StringLength(20)]
        [Unicode(false)]
        public string? Phone { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Email { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string? Sex { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [InverseProperty(nameof(AppointmentTbl.Owner))]
        public virtual ICollection<AppointmentTbl> AppointmentTbls { get; set; }
        [InverseProperty(nameof(PatientTbl.Owner))]
        public virtual ICollection<PatientTbl> PatientTbls { get; set; }
        [InverseProperty(nameof(UserTbl.Owner))]
        public virtual ICollection<UserTbl> UserTbls { get; set; }
    }
}
