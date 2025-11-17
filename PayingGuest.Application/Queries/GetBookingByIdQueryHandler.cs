using AutoMapper;
using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDto?>
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public GetBookingByIdQueryHandler(IUnitOfWork context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BookingDto?> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == request.BookingId);

            return entity is null ? null : _mapper.Map<BookingDto>(entity);
        }
    }
}
