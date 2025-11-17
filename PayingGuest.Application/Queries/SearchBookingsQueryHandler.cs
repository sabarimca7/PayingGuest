using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Domain.Interfaces;


namespace PayingGuest.Application.Queries
{
    public class SearchBookingsQueryHandler : IRequestHandler<SearchBookingsQuery, List<BookingDto>>
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public SearchBookingsQueryHandler(IUnitOfWork context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<BookingDto>> Handle(SearchBookingsQuery request, CancellationToken cancellationToken)
        {


            return await _context.Bookings
                .GetAllAsync()
                .ContinueWith(task => task.Result
                    .AsQueryable()
                    .ProjectTo<BookingDto>(_mapper.ConfigurationProvider)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList(), cancellationToken);

        }
    }
}
