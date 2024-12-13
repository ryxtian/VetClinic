using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("UserTbl")]
    public partial class UserTbl
    {
        [Key]
        public long User_ID { get; set; }
        [Required]
        [Unicode(false)]
        public string? Password { get; set; }
        [Required]
        [Unicode(false)]
        public string? ConfirmPassword { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateRegistered { get; set; }
        public short? IsDeleted { get; set; }
        public long? Owner_ID { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string? Roles { get; set; }
        public short? IsActivated { get; set; }

        [ForeignKey(nameof(Owner_ID))]
        [InverseProperty(nameof(OwnerTbl.UserTbls))]
        public virtual OwnerTbl? Owner { get; set; }
    }
}
