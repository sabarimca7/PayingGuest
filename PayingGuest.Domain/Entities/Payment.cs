using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayingGuest.Domain.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        // 🔗 Foreign Keys
        public int BookingId { get; set; }
        //public int UserId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; }

        // ================= PAYMENT DETAILS =================

        [Required]
        [MaxLength(50)]
        public string? PaymentNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string? PaymentType { get; set; }      // Rent / Deposit / Maintenance

        [Required]
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }    // UPI / Cash / Bank Transfer

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }
        public DateTime DueDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Status { get; set; }            // Paid / Pending

        // ================= TRANSACTION INFO =================

        [MaxLength(100)]
        public string? TransactionId { get; set; }

        [MaxLength(100)]
        public string? BankReference { get; set; }

        [MaxLength(100)]
        public string? UpiReference { get; set; }

        [MaxLength(200)]
        public string Remarks { get; set; }

        [MaxLength(50)]
        public string ReceiptNumber { get; set; }

        [MaxLength(50)]
        public string ReceivedBy { get; set; }

        // ================= AUDIT =================

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    
    }
}
