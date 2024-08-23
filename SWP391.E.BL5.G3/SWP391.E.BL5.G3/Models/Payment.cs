using System;
using System.Collections.Generic;

namespace SWP391.E.BL5.G3.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionId { get; set; } = null!;
        public int Status { get; set; }
        public string? ResponseCode { get; set; }
        public string? Message { get; set; }

        public virtual Booking Booking { get; set; } = null!;
    }
}
