using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    [Table("Maintenance", Schema = "PG")]
    public class Maintenance
    {
        [Key]
        public int MaintenanceId { get; set; }

        public int PropertyId { get; set; }
        public int RoomId { get; set; }
        public int BedId { get; set; }

        public string IssueType { get; set; }
        public string Description { get; set; }

        public string Status { get; set; }
        public int ReportedBy { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
