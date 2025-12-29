using MediatR;
using PayingGuest.Application.Interfaces;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class CreatePaymentCommandHandler
       : IRequestHandler<CreatePaymentCommand, int>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPaymentRepository _paymentRepository;

        public CreatePaymentCommandHandler(
            IBookingRepository bookingRepository,
            IPaymentRepository paymentRepository)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<int> Handle(
            CreatePaymentCommand request,
            CancellationToken cancellationToken)
        {
            // 🔒 Validate booking exists
            var booking = await _bookingRepository
                .GetByIdAsync(request.BookingId);

            if (booking == null)
                throw new Exception("Booking not found");

            var payment = new Payment
            {
                BookingId = request.BookingId,
                PaymentNumber = "PAY" + DateTime.UtcNow.Ticks,
                PaymentMethod = request.PaymentMethod,
                Amount = request.TotalAmount,
                PaymentType = "Monthly Rent",
                PaymentDate = DateTime.UtcNow,
                Status = "Paid"
            };

            await _paymentRepository.AddAsync(payment);

            return payment.PaymentId;
        }
    }
}