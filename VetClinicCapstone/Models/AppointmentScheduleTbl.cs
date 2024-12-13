using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VetClinicCapstone.Models
{
    [Table("AppointmentScheduleTbl")]
    public partial class AppointmentScheduleTbl
    {
        [Key]
        public long ID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Date { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? Title { get; set; }
        [StringLength(250)]
        [Unicode(false)]
        public string? DateOfEvents { get; set; }
    }
}
