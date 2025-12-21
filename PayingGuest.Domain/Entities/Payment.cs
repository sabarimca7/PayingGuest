using System;

namespace PayingGuest.Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int BookingId { get; set; }

        // 🔗 Navigation Property
        public Booking Booking { get; set; }

        public int UserId { get; set; }

        public string PaymentNumber { get; set; }

        public string PaymentType { get; set; }

        public string PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string Status { get; set; }

        public string TransactionId { get; set; }

        public string BankReference { get; set; }

        public string UpiReference { get; set; }

        public string Remarks { get; set; }

        public string ReceiptNumber { get; set; }

        public string ReceivedBy { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public User User { get; set; }

 
    }
}
