using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("ServiceTbl")]
    public partial class ServiceTbl
    {
        [Key]
        public long Service_ID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? ServiceName { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? ServicePrice { get; set; }
        public long? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public long? DeletedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeletedAt { get; set; }
        public short? IsDeleted { get; set; }
    }
}
