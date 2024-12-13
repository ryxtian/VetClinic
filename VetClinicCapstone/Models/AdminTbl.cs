using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("AdminTbl")]
    public partial class AdminTbl
    {
        [Key]
        public long Admin_ID { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? Username { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? Password { get; set; }
        public long AdminInfo_ID { get; set; }
        public short? IsDeleted { get; set; }

        [ForeignKey(nameof(AdminInfo_ID))]
        [InverseProperty(nameof(AdminInfoTbl.AdminTbls))]
        public virtual AdminInfoTbl? AdminInfo { get; set; }
    }
}
