using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("EventsTbl")]
    public partial class EventsTbl
    {
        [Key]
        public long Event_ID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? TItle { get; set; }
        [StringLength(200)]
        [Unicode(false)]
        public string? Dates { get; set; }
    }
}
